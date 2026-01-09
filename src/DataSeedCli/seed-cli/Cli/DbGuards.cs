using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace seed_cli.Cli
{
    internal static class DbGuards
    {
        // Read-only probe: open connection + SELECT 1
        public static void ProbeConnectivityReadOnly(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is missing.", nameof(connectionString));

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT 1";
                    cmd.ExecuteScalar();
                }
            }
        }

        // Schema-agnostic: returns TABLE_NAME values for BASE TABLES (case-insensitive set)
        public static HashSet<string> GetBaseTableNames(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is missing.", nameof(connectionString));

            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = @"
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);
                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                names.Add(name);
                            }
                        }
                    }
                }
            }

            return names;
        }

        public static string[] GetMissingTables(HashSet<string> existingTableNames, IEnumerable<string> requiredTables)
        {
            if (existingTableNames == null) throw new ArgumentNullException(nameof(existingTableNames));
            if (requiredTables == null) throw new ArgumentNullException(nameof(requiredTables));

            var missing = new List<string>();

            foreach (var required in requiredTables)
            {
                if (string.IsNullOrWhiteSpace(required)) continue;

                if (!existingTableNames.Contains(required))
                {
                    missing.Add(required);
                }
            }

            missing.Sort(StringComparer.OrdinalIgnoreCase);
            return missing.ToArray();
        }
    }
}
