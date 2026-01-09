using System;

namespace seed_cli.Cli
{
    internal static class CliParser
    {
        // Slice-only parser:
        // - Supports: --help, --initial-password <value>, --identity-connection <value>, --musicstore-connection <value>
        // - Treats connection args as literal connection strings (no name resolution, no config/env reading).
        public static CliArgs Parse(string[] args)
        {
            var result = new CliArgs();

            if (args == null || args.Length == 0)
                return result;

            for (int i = 0; i < args.Length; i++)
            {
                var token = args[i] ?? string.Empty;

                if (EqualsFlag(token, "--help") || EqualsFlag(token, "-h") || EqualsFlag(token, "/?"))
                {
                    result.HelpRequested = true;
                    continue;
                }

                if (EqualsFlag(token, "--initial-password"))
                {
                    result.InitialPassword = ReadValueOrNull(args, ref i);
                    continue;
                }

                if (EqualsFlag(token, "--identity-connection"))
                {
                    result.IdentityConnection = ReadValueOrNull(args, ref i);
                    continue;
                }

                if (EqualsFlag(token, "--musicstore-connection"))
                {
                    result.MusicStoreConnection = ReadValueOrNull(args, ref i);
                    continue;
                }

                // Unknown flags/args: ignored for this slice (no behavior specified).
            }

            return result;
        }

        private static bool EqualsFlag(string token, string flag)
            => string.Equals(token, flag, StringComparison.OrdinalIgnoreCase);

        private static string ReadValueOrNull(string[] args, ref int index)
        {
            // Expect a value next.
            if (index + 1 >= args.Length)
                return null;

            var candidate = args[index + 1];

            // If next token looks like a flag, treat as missing value.
            if (candidate != null && candidate.StartsWith("--", StringComparison.Ordinal))
                return null;

            index++; // consume value
            return candidate;
        }
    }
}

