using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core
{
    public class MigrationOperations
    {
        private readonly string _connectionString;
        private readonly string _connectionLegacy;

        public MigrationOperations(string connectionString, string connectionLegacy)
        {
            _connectionString = connectionString;
            _connectionLegacy = connectionLegacy;
        }


        public IList<WebUser> MigrateLegacyUsers()
        {
            using (var sqlConnection = new SqlConnection(_connectionLegacy))
            {
                sqlConnection.Open();

                using (var getLegacyUsers = new SqlCommand())
                {
                    getLegacyUsers.Connection = sqlConnection;
                    getLegacyUsers.CommandType = CommandType.StoredProcedure;
                    getLegacyUsers.CommandText = "MigrateLegacyUsers";
                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();

                            using (var reader = getLegacyUsers.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    using (var updater = new SqlCommand())
                                    {


                                    }
                                }
                            }
                        }
                    }

                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            int idWebinarFile;
            using (var sqlConnection = new SqlConnection(_connectionLegacy))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {
                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "SELECT [idWebinarFile] ,[idWebinar] ,[fileLocation] ,[fileDesc] ,[myFilename] FROM [dbo].[WebinarFile]";

                    using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                    {
                        sqlUpdateConnection.Open();

                        using (var reader = getLegacyWebinars.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idWebinarFile = reader.GetInt32(0);
                                ////idWebinar = reader.GetInt32(1);
                                //fileLocation = reader.GetString(2);
                                //fileDesc = reader.GetString(3);
                                //myFilename = reader.GetString(4);
                                using (var updateWebUserFromLegacy = new SqlCommand())
                                {
                                    updateWebUserFromLegacy.Connection = sqlUpdateConnection;
                                    updateWebUserFromLegacy.CommandType = CommandType.StoredProcedure;
                                    string idUserFromLegacy;
                                    updateWebUserFromLegacy.CommandText = "UpdateWebUserFromLegacy";

                                    using (var findUsersFromLegacy = updateWebUserFromLegacy.ExecuteReader())
                                    {
                                        if (!findUsersFromLegacy.HasRows)
                                        {
                                            using (var wfInserter = new SqlCommand())
                                            {
                                                {
                                                    wfInserter.Connection = sqlUpdateConnection;

                                                    wfInserter.CommandType = CommandType.StoredProcedure;
                                                    wfInserter.CommandText = "FindUsersFromLegacy";

                                                    try
                                                    {

                                                        wfInserter.ExecuteNonQuery();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        //Console.WriteLine("failed to insert " + idWebinar);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public IList<WebUser> GetLegacyUsers()
        {
            throw new NotImplementedException();
        }
    }
}
