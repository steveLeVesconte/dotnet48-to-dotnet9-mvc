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

        static int Main(string[] args)
        {
            var logger = new ConsoleLogger();

            // Step 1: Parse CLI args (always-verbose)
            CliArgs cli;
            using (logger.Step(dbTarget: "CLI", action: "Parse arguments", out var step1))
            {
                cli = CliParser.Parse(args);
                step1.Ok();
            }

            // Help behavior: --help prints usage and exits with code 0.
            if (cli.HelpRequested)
            {
                Usage.Print();
                return ExitSuccess;
            }

            // Step 2: Validate required inputs (always-verbose)
            string[] missing;
            using (logger.Step(dbTarget: "CLI", action: "Validate required inputs", out var step2))
            {
                missing = CliValidator.GetMissingRequiredInputs(cli);
                if (missing.Length == 0)
                {
                    step2.Ok();
                }
                else
                {
                    step2.Fail();
                }
            }

            if (missing.Length > 0)
            {
                // Missing required inputs → exit code 1 and logs which inputs are missing (no secrets).
                logger.Info("Missing required inputs: " + string.Join(", ", missing));

                // --help is also shown after missing-argument errors.
                Usage.Print();

                return ExitMissingRequiredInputs;
            }

            // Step 3: Log presence-only for secret inputs (never echo raw values)
            using (logger.Step(dbTarget: "CLI", action: "Log secret input presence (safe)", out var step3))
            {
                logger.Info($"Arg presence: --initial-password={(cli.InitialPasswordProvided ? "provided" : "missing")}");
                logger.Info($"Arg presence: --identity-connection={(cli.IdentityConnectionProvided ? "provided" : "missing")}");
                logger.Info($"Arg presence: --musicstore-connection={(cli.MusicStoreConnectionProvided ? "provided" : "missing")}");
                step3.Ok();
            }

            // Step 6 — Identity DB connectivity check
            using (logger.Step(dbTarget: "Identity", action: "Connectivity check (open + SELECT 1)", out var step6))
            {
                try
                {
                    DbGuards.ProbeConnectivityReadOnly(cli.IdentityConnection);
                    logger.Info("Identity DB reachable OK");
                    step6.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"ERROR stepName=\"Identity DB connectivity\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step6.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 7 — MusicStore DB connectivity check
            using (logger.Step(dbTarget: "MusicStore", action: "Connectivity check (open + SELECT 1)", out var step7))
            {
                try
                {
                    DbGuards.ProbeConnectivityReadOnly(cli.MusicStoreConnection);
                    logger.Info("MusicStore DB reachable OK");
                    step7.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"ERROR stepName=\"MusicStore DB connectivity\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step7.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 8 — Identity DB required schema check (Identity tables exist)
            using (logger.Step(dbTarget: "Identity", action: "Schema check (required Identity tables exist)", out var step8))
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
                        logger.Info($"Identity DB missing required tables: count={missingIdentity.Length} list=[{string.Join(", ", missingIdentity)}]");
                        step8.Fail();
                        return ExitPreconditionsFailed;
                    }

                    step8.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"ERROR stepName=\"Identity DB schema check\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step8.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Step 9 — MusicStore DB required schema check (core tables exist)
            using (logger.Step(dbTarget: "MusicStore", action: "Schema check (required core tables exist)", out var step9))
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
                        logger.Info($"MusicStore DB missing required tables: count={missingMusicStore.Length} list=[{string.Join(", ", missingMusicStore)}]");
                        step9.Fail();
                        return ExitPreconditionsFailed;
                    }

                    step9.Ok();
                }
                catch (Exception ex)
                {
                    var safeMsg = Secrets.SanitizeExceptionMessage(ex.Message, cli);
                    logger.Info($"ERROR stepName=\"MusicStore DB schema check\" exceptionType={ex.GetType().FullName} message=\"{safeMsg}\"");
                    step9.Fail();
                    return ExitPreconditionsFailed;
                }
            }

            // Suggested chunk-level success log (optional)
            logger.Info("STEP Preconditions connectivity/schema OK (identityOk=true musicstoreOk=true)");

            // End of this slice: no writes
            return ExitSuccess;
        }
    }
}
