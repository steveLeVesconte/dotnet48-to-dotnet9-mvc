using System;
using seed_cli.Cli;

namespace seed_cli
{
    internal class Program
    {
        // Exit codes per slice contract
        private const int ExitSuccess = 0;
        private const int ExitMissingRequiredInputs = 1;
        private const int ExitPreconditionsFailed = 2;

        // Chunk-3-Specs.md adds an additional precondition exit code for freshness violations.
        private const int ExitFreshnessGatesViolated = 3;

        static int Main(string[] args)
        {
            var logger = new ConsoleLogger();

            // Step 1: Parse CLI args (always-verbose)
            CliArgs cli;
            using (logger.Step(dbTarget: "CLI", action: "Parse arguments", out var step))
            {
                cli = CliParser.Parse(args);
                step.Ok();
            }

            // Help behavior: --help prints usage and exits with code 0.
            if (cli.HelpRequested)
            {
                Usage.Print();
                return ExitSuccess;
            }

            // Step 2: Validate required inputs (always-verbose)
            string[] missing;
            using (logger.Step(dbTarget: "CLI", action: "Validate required inputs", out var step))
            {
                missing = CliValidator.GetMissingRequiredInputs(cli);
                if (missing.Length == 0)
                {
                    step.Ok();
                }
                else
                {
                    step.Fail();
                }
            }

            if (missing.Length > 0)
            {
                // Missing required inputs → exit code 1 and logs which inputs are missing (no secrets).
                logger.Info("    Missing required inputs: " + string.Join(", ", missing));
                // --help is also shown after missing-argument errors.
                Usage.Print();
                return ExitMissingRequiredInputs;
            }

            // Step 3: Log presence-only for secret inputs (never echo raw values)
            using (logger.Step(dbTarget: "CLI", action: "Log secret input presence (safe)", out var step3))
            {
                logger.Info($"    Arg presence: --initial-password={(cli.InitialPasswordProvided ? "provided" : "missing")}");
                logger.Info($"    Arg presence: --identity-connection={(cli.IdentityConnectionProvided ? "provided" : "missing")}");
                logger.Info($"    Arg presence: --musicstore-connection={(cli.MusicStoreConnectionProvided ? "provided" : "missing")}");
                step3.Ok();
            }

            // Step 4 — Identity DB connectivity check
            using (logger.Step(dbTarget: "Identity", action: "Connectivity check (open + SELECT 1)", out var step))
            {
                try
                {
                    DbGuards.ProbeConnectivityReadOnly(cli.IdentityConnection);
                    logger.Info("    Identity DB reachable OK");
                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"Identity DB connectivity\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 5 — MusicStore DB connectivity check
            using (logger.Step(dbTarget: "MusicStore", action: "Connectivity check (open + SELECT 1)", out var step))
            {
                try
                {
                    DbGuards.ProbeConnectivityReadOnly(cli.MusicStoreConnection);
                    logger.Info("    MusicStore DB reachable OK");
                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"MusicStore DB connectivity\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 6 — Identity DB required schema check (Identity tables exist)
            using (logger.Step(dbTarget: "Identity", action: "Schema check (required Identity tables exist)", out var step))
            {
                try
                {
                    var existing = DbGuards.GetBaseTableNames(cli.IdentityConnection);
                    var requiredIdentityTables = new[]
                    {
            "AspNetUsers",
            "AspNetRoles",
            "AspNetUserClaims",
            "AspNetUserLogins",
            "AspNetUserRoles"
          };

                    var missingIdentity = DbGuards.GetMissingTables(existing, requiredIdentityTables);
                    if (missingIdentity.Length > 0)
                    {
                        logger.Info($"    Identity DB missing required tables: count={missingIdentity.Length} list=[{string.Join(", ", missingIdentity)}]");
                        step.Fail();
                        return ExitPreconditionsFailed;
                    }

                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"Identity DB schema check\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 7 — MusicStore DB required schema check (core tables exist)
            using (logger.Step(dbTarget: "MusicStore", action: "Schema check (required core tables exist)", out var step))
            {
                try
                {
                    var existing = DbGuards.GetBaseTableNames(cli.MusicStoreConnection);
                    var requiredMusicStoreTables = new[]
                    {
            "Albums",
            "Artists",
            "Genres",
            "Orders",
            "OrderDetails",
            "Carts"
          };

                    var missingMusicStore = DbGuards.GetMissingTables(existing, requiredMusicStoreTables);
                    if (missingMusicStore.Length > 0)
                    {
                        logger.Info($"    MusicStore DB missing required tables: count={missingMusicStore.Length} list=[{string.Join(", ", missingMusicStore)}]");
                        step.Fail();
                        return ExitPreconditionsFailed;
                    }

                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"MusicStore DB schema check\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 8 — Catalog thresholds check (Albums>=460, Artists>=302, Genres>=14)
            using (logger.Step(dbTarget: "MusicStore", action: "Catalog thresholds check (Albums/Artists/Genres counts)", out var step))
            {
                try
                {
                    var albums = DbGuards.CountTableRows(cli.MusicStoreConnection, "Albums");
                    var artists = DbGuards.CountTableRows(cli.MusicStoreConnection, "Artists");
                    var genres = DbGuards.CountTableRows(cli.MusicStoreConnection, "Genres");

                    logger.Info($"    Catalog counts: Albums={albums} Artists={artists} Genres={genres}");

                    var below =
                      (albums < 460) ||
                      (artists < 302) ||
                      (genres < 14);

                    if (below)
                    {
                        logger.Info("    Catalog thresholds NOT met: required Albums>=460, Artists>=302, Genres>=14");
                        step.Fail();
                        return ExitPreconditionsFailed; // exit code 2 per Chunk-3-Specs.md
                    }

                    logger.Info($"    Catalog counts OK (Albums={albums}, Artists={artists}, Genres={genres})");
                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"Catalog thresholds\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 9 — Freshness gates check (AspNetUsers==1; Orders==0; OrderDetails==0; Carts==0)
            using (logger.Step(dbTarget: "Both", action: "Freshness gates check (AspNetUsers/Orders/OrderDetails/Carts)", out var step))
            {
                try
                {
                    var aspNetUsers = DbGuards.CountTableRows(cli.IdentityConnection, "AspNetUsers");
                    var orders = DbGuards.CountTableRows(cli.MusicStoreConnection, "Orders");
                    var orderDetails = DbGuards.CountTableRows(cli.MusicStoreConnection, "OrderDetails");
                    var carts = DbGuards.CountTableRows(cli.MusicStoreConnection, "Carts");

                    logger.Info($"    Freshness counts: AspNetUsers={aspNetUsers} Orders={orders} OrderDetails={orderDetails} Carts={carts}");

                    var violated =
                      (aspNetUsers != 1) ||
                      (orders != 0) ||
                      (orderDetails != 0) ||
                      (carts != 0);

                    if (violated)
                    {
                        logger.Info("    Freshness gates VIOLATED: expected AspNetUsers==1 and Orders==0 and OrderDetails==0 and Carts==0");
                        step.Fail();
                        return ExitFreshnessGatesViolated; // exit code 3 per Chunk-3-Specs.md
                    }

                    logger.Info("   Freshness gates OK");
                    step.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"    ERROR stepName=\"Freshness gates\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Suggested chunk-level success log (optional)
            logger.Info("    STEP Preconditions connectivity/schema OK (identityOk=true musicstoreOk=true)");

            // End of this slice: no writes
            return ExitSuccess;
        }
    }
}
