using System;
using seed_cli.Cli;

namespace seed_cli
{
    internal class Program
    {
        // Exit codes per slice contract
        private const int ExitSuccess = 0;
        private const int ExitMissingRequiredInputs = 1;

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

            // End of slice: do not implement beyond First_Coding_Chunk.md
            return ExitSuccess;
        }
    }
}
