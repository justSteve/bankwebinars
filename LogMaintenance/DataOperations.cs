using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

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

        public string UploadLog4Net()
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getUnParsedLogs = new SqlCommand())
                {
                    getUnParsedLogs.Connection = sqlConnection;
                    getUnParsedLogs.CommandType = CommandType.Text;
                    getUnParsedLogs.CommandText =
                        " SELECT * FROM dbo.Log4Net WHERE Logger LIKE '%|%'; ";
                    DateTime DateTime;
                    var Thread = "";
                    var Level = "";
                    var Logger = "";
                    var Message = "";
                    var Exception = "";
                    var ipAddress = "";
                    var idSession = "";
                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();

                            using (var reader = getUnParsedLogs.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //reader.GetInt32(0), reader.GetDecimal(1)));

                                    DateTime = reader.GetDateTime(0);
                                    Thread = reader.GetString(1);
                                    Level = reader.GetString(2);
                                    Logger = reader.GetString(3).Split('|')[2];
                                    Message = reader.GetString(4);
                                    Exception = reader.GetString(5);
                                    ipAddress = reader.GetString(3).Split('|')[0];
                                    idSession = reader.GetString(3).Split('|')[1];

                                    InsertUserSession(DateTime, Thread, Level, Logger, Message, Exception, ipAddress,
                                        idSession);
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

        private string InsertUserSession(DateTime dateTime, string thread
            , string level, string logger
            , string message, string exception
            , string ipAddress, string idSession)
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                // new SqlCommand("GetAuditNotes", sqlConnection))

                using (var getUnParsedLogs = new SqlCommand())
                {
                    getUnParsedLogs.Connection = sqlConnection;
                    getUnParsedLogs.CommandType = CommandType.Text;
                    getUnParsedLogs.CommandText =
                        " SELECT * FROM dbo.Log4Net WHERE Logger LIKE '%|%'; ";

                    var dateTimeParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.DateTime,
                        ParameterName = "@dateTime",
                        Value = dateTime
                    };
                    var threadParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@thread",
                        Value = thread
                    };

                    var levelParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@level",
                        Value = level
                    };


                    var loggerParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@logger",
                        Value = logger
                    };

                    var messageParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@message",
                        Value = message
                    };

                    var ipParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@ipAddress",
                        Value = ipAddress
                    };
                    var exceptionParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@exception",
                        Value = exception
                    };
                    var idSessionParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@idSession",
                        Value = idSession
                    };

                    getUnParsedLogs.Parameters.Add(loggerParam);
                    getUnParsedLogs.Parameters.Add(ipParam);
                    getUnParsedLogs.Parameters.Add(dateTimeParam);
                    getUnParsedLogs.Parameters.Add(threadParam);
                    getUnParsedLogs.Parameters.Add(levelParam);
                    getUnParsedLogs.Parameters.Add(messageParam);
                    getUnParsedLogs.Parameters.Add(exceptionParam);
                    getUnParsedLogs.Parameters.Add(idSessionParam);
                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();

                            getUnParsedLogs.ExecuteNonQuery();
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


        public string UploadIIS()
        {
            var returnLable = new StringBuilder();
            var uniqueCookie = new StringBuilder();

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                var updateSiteLog = "";
                using (var getUnParsedLogs = new SqlCommand())
                {
                    getUnParsedLogs.Connection = sqlConnection;
                    getUnParsedLogs.CommandType = CommandType.Text;
                    getUnParsedLogs.CommandText =
                        " SELECT * FROM dbo.SiteLog WHERE time != '' ";
                    try
                    {

                        using (var reader = getUnParsedLogs.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var getSessionId = "";
                                if (reader[0] != null)
                                {
                                    var sessionId = "";

                                    var date = reader.GetDateTime(0);
                                    var time = reader.GetDateTime(1);
                                    var s_Sitename = reader.GetString(2) ?? "";
                                    var cs_Method = reader.GetString(3) ?? "";
                                    var cs_Uri_Stem = reader.GetString(4) ?? "";
                                    var cs_Uri_Query = reader[5] ?? "";
                                    var s_Port = reader.GetInt32(6);
                                    var cs_Username = reader[7] ?? "";
                                    var c_Ip = reader.GetString(8) ?? "";
                                    var cs_UserAgent = reader[9] ?? "";
                                    var cs_Cookie = reader[10] ?? "";
                                    var cs_Referer = reader[11] ?? "";
                                    var cs_Host = reader.GetString(12) ?? "";
                                    var sc_Status = reader.GetInt32(13);
                                    var sc_Substatus = reader.GetInt32(14);
                                    var sc_Win32Status = reader.GetInt32(15);
                                    var sc_Bytes = reader.GetInt32(16);
                                    var cs_Bytes = reader.GetInt32(17);
                                    var time_Taken = reader.GetInt32(18);
                                    var Id = reader.GetInt32(19);

                                    var numOfSemiPlus = Regex.Match(cs_Cookie.ToString(), ";+").Length;
                                    var numOfEquals = Regex.Match(cs_Cookie.ToString(), "=").Length;

                                    if (cs_Cookie.ToString().Contains("~"))
                                    {
                                        var a = 1;
                                    }
                                    cs_Cookie = cs_Cookie.ToString().Replace(";+", "~");

                                    var i = cs_Cookie.ToString().Split('~');
                                    if (i.Length > 0)
                                    {
                                        foreach (var cookie in i)
                                        {
                                            if (cookie.Length > 0 && cookie != "...")
                                            {
                                                var name = cookie.Split('=')[0];
                                                var value = cookie.Split('=')[1];

                                                if (!uniqueCookie.ToString().Contains(name))
                                                {
                                                    uniqueCookie.AppendLine(name);
                                                }
                                                if (name == "ASP.NET_SessionId")
                                                    sessionId = value;
                                            }
                                        }
                                    }
                                    updateSiteLog =
                                        "INSERT INTO[dbo].[UserSession](idSiteLog,[IISSession],[date],[s-Sitename],[cs-Method],[cs-Uri-Stem],[cs-Uri-Query],[cs-Username],[c-Ip],[cs(UserAgent)],[cs(Cookie)],[cs(Referer)],[cs-Host],[sc-Status],[sc-Substatus],[s-Port],[sc-Win32Status])" +
                                        "VALUES(" + Id + ",'" + sessionId + "','" + date + "','" +
                                        s_Sitename + "','" + cs_Method + "','" + cs_Uri_Stem + "','" +
                                        cs_Uri_Query + "','" + cs_Username + "','" + c_Ip + "','" +
                                        cs_UserAgent + "','" + cs_Cookie + "','" + cs_Referer + "','" +
                                        cs_Host + "'," + sc_Status + "," + sc_Substatus + "," + s_Port +
                                        "," + sc_Win32Status + ")";

                                    var insertAttempt = InsertUserSessionRecord(updateSiteLog.ToString());
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        returnLable.AppendLine("on parsing: " + e.Message);
                    }
                }
                returnLable.AppendLine(uniqueCookie.ToString());
                returnLable.AppendLine("");
                using (var insertParsedLogs = new SqlCommand())
                {
                    insertParsedLogs.Connection = sqlConnection;
                    insertParsedLogs.CommandType = CommandType.Text;
                    insertParsedLogs.CommandText =
                        " update dbo.SiteLog set time = '' WHERE time != '' ";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            insertParsedLogs.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        returnLable.AppendLine("on inserting: " + ex.Message);
                    }
                }
            }
            return returnLable.ToString();
        }

        private string InsertUserSessionRecord(string updateSiteLog)
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var insertParsedLogs = new SqlCommand())
                {
                    insertParsedLogs.Connection = sqlConnection;
                    insertParsedLogs.CommandType = CommandType.Text;
                    insertParsedLogs.CommandText =
                        updateSiteLog;

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            insertParsedLogs.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        returnLable = ("on inserting: " + ex.Message);
                    }
                }
                return returnLable;
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
