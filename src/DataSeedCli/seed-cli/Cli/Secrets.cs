using System;

namespace seed_cli.Cli
{
    internal static class Secrets
    {
        // Central list of secret-bearing values to sanitize from exception messages.
        // Note: We never log raw secrets, and we do not attempt to parse credentials.
        public static string SanitizeExceptionMessage(string message, CliArgs args)
        {
            if (string.IsNullOrEmpty(message)) return message;

            var sanitized = message;

            sanitized = ReplaceIfPresent(sanitized, args?.InitialPassword);
            sanitized = ReplaceIfPresent(sanitized, args?.IdentityConnection);
            sanitized = ReplaceIfPresent(sanitized, args?.MusicStoreConnection);

            return sanitized;
        }

        private static string ReplaceIfPresent(string input, string secret)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(secret)) return input;

            // Replace exact secret occurrences only (avoid overreach).
            return input.Replace(secret, "<redacted>");
        }
    }
}

