using System.Collections.Generic;

namespace seed_cli.Cli
{
    internal static class CliValidator
    {
        public static string[] GetMissingRequiredInputs(CliArgs args)
        {
            var missing = new List<string>();

            // Required unless help
            if (!args.InitialPasswordProvided) missing.Add("--initial-password");
            if (!args.IdentityConnectionProvided) missing.Add("--identity-connection");
            if (!args.MusicStoreConnectionProvided) missing.Add("--musicstore-connection");

            return missing.ToArray();
        }
    }
}
