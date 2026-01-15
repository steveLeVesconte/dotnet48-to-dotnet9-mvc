using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

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

        // Chunk 3 helper: Count rows in a known base table, safely.
        public static int CountTableRows(string connectionString, string tableName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string is missing.", nameof(connectionString));

            var safeTable = ToSafeSqlIdentifier(tableName);
            var sql = $"SELECT COUNT(1) FROM [dbo].[{safeTable}];";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;

                    var result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                        return 0;

                    // COUNT(*) in SQL Server returns bigint; convert carefully.
                    return Convert.ToInt32(result);
                }
            }
        }

        // Only allow simple identifiers to avoid SQL injection when embedding identifiers.
        private static string ToSafeSqlIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Identifier is missing.", nameof(identifier));

            // Letters, numbers, underscore only.
            // (All table names used in this slice match this.)
            if (!Regex.IsMatch(identifier, @"^[A-Za-z0-9_]+$"))
                throw new ArgumentException("Identifier contains invalid characters.", nameof(identifier));

            return identifier;
        }
    }
}
