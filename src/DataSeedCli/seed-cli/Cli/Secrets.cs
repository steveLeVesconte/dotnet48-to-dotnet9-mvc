using System;
using System.Text.RegularExpressions;

namespace seed_cli.Cli
{
    internal static class Secrets
    {
        // Regex to redact SQL Server login identifiers from exception messages.
        // Examples:
        //   Login failed for user 'DOMAIN\User'.
        //   Login failed for user 'sa'.
        private static readonly Regex SqlLoginRegex =
            new Regex(
                @"Login failed for user\s+'[^']+'",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        // Regex to redact SQL Server name from exception messages.
        // Example:
        //   Cannot open database "xMvcMusicStoreUsers" requested...
        private static readonly Regex SqlServerNameRegex =
            new Regex(
                @"Cannot open database\s+(['""])(?<db>.*?)\1\s+requested by",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        // Central list of secret-bearing values to sanitize from exception messages.
        // Note: We never log raw secrets, and we do not attempt to parse credentials.
        public static string SanitizeExceptionMessage(string message, CliArgs args)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            var sanitized = message;

            // Exact-value redaction (strongest guarantee)
            sanitized = ReplaceIfPresent(sanitized, args?.InitialPassword);
            sanitized = ReplaceIfPresent(sanitized, args?.IdentityConnection);
            sanitized = ReplaceIfPresent(sanitized, args?.MusicStoreConnection);

            // Targeted SQL login identifier redaction (PII / environment identifiers)
            sanitized = SqlLoginRegex.Replace(
                sanitized,
                "Login failed for user '<redacted-user>'");

            // Targeted SQL Server identifier redaction (PII / environment identifiers)
            sanitized = SqlServerNameRegex.Replace(
                sanitized,
                "Cannot open database <redacted-db> requested by");

            return sanitized;
        }

        private static string ReplaceIfPresent(string input, string secret)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(secret))
                return input;

            // Replace exact secret occurrences only (avoid overreach).
            return input.Replace(secret, "<redacted>");
        }
    }
}


