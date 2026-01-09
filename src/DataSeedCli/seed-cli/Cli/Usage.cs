using System;

namespace seed_cli.Cli
{
    internal static class Usage
    {
        public static void Print()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  seed-cli --initial-password \"<password>\" --identity-connection \"<connection string>\" --musicstore-connection \"<connection string>\"");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --help                         Show this help and exit (code 0).");
            Console.WriteLine("  --initial-password             Required unless --help.");
            Console.WriteLine("  --identity-connection          Required unless --help. Treated as a literal connection string from CLI args only.");
            Console.WriteLine("  --musicstore-connection        Required unless --help. Treated as a literal connection string from CLI args only.");
            Console.WriteLine();
            Console.WriteLine("Exit codes:");
            Console.WriteLine("  0  Success");
            Console.WriteLine("  1  Missing required input(s)");
        }
    }
}
