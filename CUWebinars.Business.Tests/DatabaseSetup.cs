using CUWebinars.Business.Tests.Properties;
using System;
using System.Data.SqlClient;

namespace CUWebinars.Business.Tests
{
    internal class DatabaseSetup
    {
        private const string MasterSchema = "Master";

        public string ConnectionString { get; set; }
        internal void InstallDatabase(string resourceName)
        {
            InstallDatabase(ConnectionString, GetScript(resourceName));
        }

        private static string GetScript(string resourceName)
        {
            switch (resourceName)
            {
                case Constants.CreateDbDefault:
                    return Resources.CreateDb;
                case Constants.CreateMemRebootDb:
                    return Resources.MemReboot;
                case Constants.TtsDatabaseResourceName:
                    return Resources.TtsDatabase;
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
            this.UninstallDatabase(ConnectionString, dbName);
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
