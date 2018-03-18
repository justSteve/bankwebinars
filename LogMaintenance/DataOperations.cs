using System;
using System.Data;
using System.Data.SqlClient;

namespace LogMaintenance
{
    public class DataOperations
    {
        private readonly string _connectionString;

        //private readonly ILogger _logger;
        //private readonly TtsConfiguration _ttsConfig;

        public DataOperations(string connectionString)
        {
            _connectionString = connectionString;
        }


        public string CheckForUnique(string code, string field)
        {
            var returnVal = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var CheckForUnique = new SqlCommand())
                {
                    var codeParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@code",
                        Value = code
                    };
                    var fieldParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@field",
                        Value = field
                    };

                    CheckForUnique.Parameters.Add(codeParameter);
                    CheckForUnique.Parameters.Add(fieldParameter);

                    CheckForUnique.Connection = sqlConnection;
                    CheckForUnique.CommandType = CommandType.StoredProcedure;
                    CheckForUnique.CommandText = "CheckForUnique";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();

                            returnVal = CheckForUnique.ExecuteScalar().ToString();

                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    return returnVal;

                }
            }
        }

        public string FindRegTypeForRateWatch(string regTypeLable)
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {
                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText =
                        "SELECT ttsLable from RateWatchImporter where rwLable = '" + regTypeLable +
                        "'";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();

                            using (var reader = getLegacyWebinars.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //reader.GetInt32(0), reader.GetDecimal(1)));
                                    returnLable = reader.GetString(0);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    return returnLable;

                }
            }
        }


        public class AuditChangedRegTypeModel
        {
            public string DateOfChange { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string By { get; set; }
            public string Note { get; set; }
        }
    }
}
