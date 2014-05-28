using CUWebinars.Business.Tests.Properties;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace CUWebinars.Business.Tests
{
    internal class DatabaseSetup
    {
        internal static string connStr = ConfigurationManager.ConnectionStrings["CUWebinarsSUTLocal"].ConnectionString;
        private const string MasterSchema = "Master";
        internal void InstallDatabase(string resourceName)
        {
            InstallDatabase(connStr, GetScript(resourceName));
        }

        private static string GetScript(string resourceName)
        {
            switch (resourceName)
            {
                case Constants.CreateDbDefault:
                    return Resources.CreateDb;
                default:
                    throw new NotSupportedException(string.Format("There's no resource script called {0}", resourceName));
            }
        }

        internal void InstallDatabase(string connectionString, string scriptToRunIn)
        {
            var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = MasterSchema };

            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    foreach (var sqlBlock in scriptToRunIn.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = sqlBlock;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        internal void UninstallDatabase(string dbName)
        {
            this.UninstallDatabase(connStr, dbName);
        }

        internal void UninstallDatabase(string connectionString, string dbName)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = MasterSchema;
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                string dropCmd = string.Format("IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}') DROP DATABASE [{0}];", dbName);

                using (var cmd = new SqlCommand(dropCmd, conn))
                    cmd.ExecuteNonQuery();
            }
        }
    }
}
