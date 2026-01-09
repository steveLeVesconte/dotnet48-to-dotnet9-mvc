namespace seed_cli.Cli
{
    internal sealed class CliArgs
    {
        public bool HelpRequested { get; set; }

        // Raw values are stored only for internal use (e.g., sanitizing exceptions).
        // They must NEVER be logged or echoed.
        public string InitialPassword { get; set; }
        public string IdentityConnection { get; set; }
        public string MusicStoreConnection { get; set; }

        public bool InitialPasswordProvided => !string.IsNullOrWhiteSpace(InitialPassword);
        public bool IdentityConnectionProvided => !string.IsNullOrWhiteSpace(IdentityConnection);
        public bool MusicStoreConnectionProvided => !string.IsNullOrWhiteSpace(MusicStoreConnection);
    }
}

