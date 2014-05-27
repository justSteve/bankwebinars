using CUWebinars.Business.Tests.Properties;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CUWebinars.Business.Tests
{
    internal class DatabaseSetup
    {
        internal static string connStr = ConfigurationManager.ConnectionStrings["CUWebinarsSUTLocal"].ConnectionString;
        internal void InstallDatabase()
        {
            InstallDatabase(connStr);
        }

        internal void InstallDatabase(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString) {InitialCatalog = "Master"};

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    var schemaSql = Resources.CreateDb;

                    foreach (var sql in schemaSql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
