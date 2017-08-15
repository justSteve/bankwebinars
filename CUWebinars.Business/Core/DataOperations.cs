using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Text;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Extensions;
using CUWebinars.Business.Services;
using Newtonsoft.Json;

namespace CUWebinars.Business.Core
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


        public string FindRegTypeForACS(string regTypeLable)
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "SELECT ttsLable from ACSImporter where acsLable = '" + regTypeLable +
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
                        LogError("FindRegTypeForACS", "Unknown Registration Type: " + getLegacyWebinars.CommandText +
                                                      " Exception.Message: " + ex.Message);
                    }



                    return returnLable;

                }
            }
        }

        public decimal GetCostOfRegtype(int idRegType)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getRegTypePriceCommand = new SqlCommand())
                {
                    var webinarIdParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@idRegType",
                        Value = idRegType
                    };

                    getRegTypePriceCommand.Connection = sqlConnection;
                    getRegTypePriceCommand.CommandType = CommandType.Text;
                    getRegTypePriceCommand.Parameters.Add(webinarIdParameter);
                    getRegTypePriceCommand.CommandText =
                        "SELECT Price FROM dbo.RegType WHERE idRegType = " + @idRegType;


                    var message = getRegTypePriceCommand.ExecuteScalar();
                    return Convert.ToDecimal(message);
                }
            }
        }


        /// <summary>
        /// This method permits affiliates to show friendly label on importer
        /// </summary>
        /// <param name="idWebinar"></param>
        /// <param name="registrationType">Friendly Lable</param>

        public int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar)
        {
            registrationType = registrationType.Replace(" (days)", "");
            registrationType = registrationType.Replace(" (months)", "");
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        " SELECT  regType.idRegtype FROM    dbo.RegType regType INNER JOIN dbo.RegTypesXref rx " +
                        " ON rx.idRegType = regType.idRegType INNER JOIN dbo.RegTypesGroups rtg " +
                        " ON rtg.idRegTypeGroup = rx.idRegTypeGroup INNER JOIN dbo.RegTypesGroupsXref rtgX " +
                        " ON rtgX.idRegTypeGroup = rtg.idRegTypeGroup WHERE   rtgX.idWebinar = " + idWebinar +
                        " AND regType.RegTypeLabel = '" + registrationType + "'";

                    command.CommandType = CommandType.Text;

                    var message = command.ExecuteScalar();
                    if (message == null)
                    {
                        LogError("GetRegTypeByLableAndWebinar",
                            "Invalid Registration Type" + registrationType + " for idWebinar " + idWebinar);
                    }

                    return Convert.ToInt32(message);
                }
            }
        }

        public USTimeZone GetTimeZoneByZipCode(string zip)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getTimeZoneCommand = new SqlCommand())
                {
                    var zipParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        ParameterName = "@zip",
                        Value = zip
                    };

                    getTimeZoneCommand.Connection = sqlConnection;
                    getTimeZoneCommand.CommandType = CommandType.Text;
                    getTimeZoneCommand.Parameters.Add(zipParameter);
                    getTimeZoneCommand.CommandText = "SELECT timezone FROM TimeZoneByZip WHERE zip = @zip";

                    string timeZoneAsString = string.Empty;

                    using (var reader = getTimeZoneCommand.ExecuteReader())
                    {
                        reader.Read(); //   There should only be 1
                        timeZoneAsString = reader.GetString(0);
                    }

                    int timeZoneAsInt;
                    USTimeZone timeZone = USTimeZone.Central;

                    if (int.TryParse(
                        timeZoneAsString,
                        NumberStyles.AllowLeadingSign,
                        CultureInfo.CurrentCulture,
                        out timeZoneAsInt))
                    {
                        timeZone = (USTimeZone)timeZoneAsInt;
                    }

                    return timeZone;
                }
            }
        }

        public bool SetFieldsConsistantWithVerifiedUser(UserAccount userAccount)
        {
            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var setFieldsVerifiedCommand = new SqlCommand())
                {
                    var idParam = new SqlParameter
                    {
                        DbType = DbType.Guid,
                        ParameterName = "@id",
                        Value = userAccount.ID
                    };

                    setFieldsVerifiedCommand.Connection = sqlConnection;
                    setFieldsVerifiedCommand.CommandType = CommandType.Text;
                    setFieldsVerifiedCommand.Parameters.Add(idParam);
                    setFieldsVerifiedCommand.CommandText =
                        "UPDATE [dbo].[UserAccounts] SET LastFailedLogin = NULL, FailedLoginCount = 0, IsAccountVerified = 1, IsAccountClosed = 0, AccountClosed = NULL, VerificationKey = NULL, VerificationPurpose = NULL, VerificationKeySent = NULL, LastFailedPasswordReset = NULL, FailedPasswordResetCount = 0, VerificationStorage = NULL WHERE [ID] = @id;";

                    numRows = setFieldsVerifiedCommand.ExecuteNonQuery();
                }
            }

            return numRows == 1;
        }

        public void LogError(string MethodSendingError, string ErrorToLog)
        {
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var errorLogger = new SqlCommand("logError", sqlConnection))
                {
                    errorLogger.CommandText =
                        "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                    errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                    errorLogger.CommandText += "'V3DataOp ErrorLogger' ,";
                    errorLogger.CommandText += "9 ,9 ,9 ,'" + MethodSendingError + "', 9 ,";
                    errorLogger.CommandText += "'" + ErrorToLog + "')";

                    errorLogger.ExecuteNonQuery();

                }
            }
        }

        public Double[] GetCostOfUpgrades(int idRegType)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getPricingsCommand = new SqlCommand())
                {
                    var webinarIdParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@idRegType",
                        Value = idRegType
                    };


                    getPricingsCommand.Connection = sqlConnection;
                    getPricingsCommand.CommandType = CommandType.Text;
                    getPricingsCommand.Parameters.Add(webinarIdParameter);
                    getPricingsCommand.CommandText =
                        "SELECT [base],[plus6],[premier] FROM [dbo].[UpgradePricing] WHERE  idRegType = @idRegType";

                    var aryReturn = new Double[3];

                    using (var reader = getPricingsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            aryReturn[0] = reader.GetDouble(0);
                            aryReturn[1] = reader.GetDouble(1);
                            aryReturn[2] = reader.GetDouble(2);

                        }
                    }
                    return aryReturn;
                }
            }
            throw new NotImplementedException();
        }

        public void AddChangedOrder(Dictionary<string, string> buildChangedOrderRow)
        {
            // creates Record in 'Changed Orders that require change to Royalties paid' table
            throw new NotImplementedException();
        }

        public string GetUserEmailByVerificationKey(string tenant, string key)
        {
            string email = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var setFieldsVerifiedCommand = new SqlCommand())
                {

                    var tenantParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        ParameterName = "@tenant",
                        Value = tenant
                    };
                    var keyParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        ParameterName = "@key",
                        Value = key
                    };

                    setFieldsVerifiedCommand.Connection = sqlConnection;
                    setFieldsVerifiedCommand.CommandType = CommandType.Text;
                    setFieldsVerifiedCommand.Parameters.Add(tenantParameter);
                    setFieldsVerifiedCommand.Parameters.Add(keyParameter);

                    setFieldsVerifiedCommand.CommandText =
                        "SELECT email FROM dbo.UserAccounts WHERE VerificationKey = '@key';";

                    using (var reader = setFieldsVerifiedCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            email = reader.GetString(0);
                        }
                    }
                    return email;
                }
            }


        }

        public void RemoveImpersonatedClaimsByCurrentAdmin(string adminUserEmail)
        {
            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCmd = new SqlCommand())
                {
                    var adminEmail = new SqlParameter
                    {
                        DbType = DbType.String,
                        ParameterName = "@AdminEmail",
                        Value = adminUserEmail
                    };

                    sqlCmd.Connection = sqlConnection;
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.Add(adminEmail);
                    sqlCmd.CommandText =
                        "DELETE FROM dbo.UserClaims WHERE Type = 'http://ttstrain.com/ws/2014/01/identity/claims/BeingImpersonated' AND Value like '%" +
                        adminUserEmail + "'";

                    numRows = sqlCmd.ExecuteNonQuery();
                }
            }

        }

        private static OrderStatus SetOrderStatus(int mkStatus)
        {
            OrderStatus setStatus = OrderStatus.Abandoned;
            // Error = 0, 
            //InProcess = 1,
            //Submitted = 2,
            //Billed = 3,
            //Paid = 4,
            //Abandoned = 5,
            //Canceled = 6,
            //AwaitingVerification = 7,
            //Unknown = 255
            switch (mkStatus)
            {
                case 1:
                    setStatus = OrderStatus.InProcess;
                    break;
                case 0:
                    setStatus = OrderStatus.Error;
                    break;
                case 2:
                    setStatus = OrderStatus.Submitted;
                    break;
                case 3:
                    setStatus = OrderStatus.Billed;
                    break;
                case 4:
                    setStatus = OrderStatus.Paid;
                    break;
                case 5:
                    setStatus = OrderStatus.Abandoned;
                    break;
                case 6:
                    setStatus = OrderStatus.Canceled;
                    break;
                case 7:
                    setStatus = OrderStatus.AwaitingVerification;
                    break;
            }
            return setStatus;
        }


        public PostEventClaim FindPostEventClaimByOnDemandCode(Order order)
        {
            var retValue = "";
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                var onDemandCodeParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.VarChar,
                    ParameterName = "@onDemandCode",
                    Value = order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).OnDemandCode
                };

                if (onDemandCodeParameter.Value == null) return null;

                using (var findClaim = new SqlCommand("FindPostEventClaimByOnDemandCode", sqlConnection))
                {
                    findClaim.Parameters.Add(onDemandCodeParameter);

                    try
                    {
                        findClaim.Connection = sqlConnection;
                        findClaim.CommandType = CommandType.StoredProcedure;

                        var result = findClaim.ExecuteScalar();
                        if (result != null)
                            retValue = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindPostEventClaimByOnDemandCode' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindPostEventClaimByOnDemandCode', 9 ,";
                            errorLogger.CommandText += "'error at FindPostEventClaimByOnDemandCode " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            PostEventClaim returnClaim = new PostEventClaim();
            var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(retValue.ToString());
            if (thisClaim != null && thisClaim.OnDemandCode != null && thisClaim.OnDemandCode ==
                order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).OnDemandCode)
            {
                returnClaim.OrderId = order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idOrder;
                returnClaim.OnDemandCode = thisClaim.OnDemandCode;
                returnClaim.ExpiryDate = thisClaim.ExpiryDate;
            }

            return returnClaim;
        }


        public PostEventClaim FindPostEventClaimByOrderId(Order order)
        {
            var retValue = false;
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                var onDemandCodeParameter = new SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@idOrder",
                    Value = order.idOrder
                };

                using (var insertWebUser = new SqlCommand("FindPostEventClaimByOrderId", sqlConnection))
                {
                    insertWebUser.Parameters.Add(onDemandCodeParameter);

                    try
                    {
                        insertWebUser.Connection = sqlConnection;
                        insertWebUser.CommandType = CommandType.StoredProcedure;

                        var result = insertWebUser.ExecuteScalar();
                        if (result == null)
                            retValue = true;
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindPostEventClaimByOrderId' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindPostEventClaimByOrderId', 9 ,";
                            errorLogger.CommandText += "'error at FindPostEventClaimByOrderId " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            PostEventClaim returnClaim = new PostEventClaim();
            var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(retValue.ToString());
            if (thisClaim.OnDemandCode ==
                order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).OnDemandCode)
            {
                returnClaim.OrderId = order.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idOrder;
                returnClaim.OnDemandCode = thisClaim.OnDemandCode;
                returnClaim.ExpiryDate = thisClaim.ExpiryDate;
            }

            return returnClaim;
        }


        public IList<PostEventClaim> FindAllPostEventClaims()
        {
            IList<PostEventClaim> retList = new List<PostEventClaim>();
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var FindAllPostEventClaims = new SqlCommand("FindAllPostEventClaims", sqlConnection))
                {
                    try
                    {
                        FindAllPostEventClaims.Connection = sqlConnection;
                        FindAllPostEventClaims.CommandType = CommandType.StoredProcedure;

                        using (var reader = FindAllPostEventClaims.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //PostEventClaim returnClaim = new PostEventClaim();
                                var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(reader[0].ToString());
                                retList.Add(thisClaim);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindAllPostEventClaims' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindAllPostEventClaims', 9 ,";
                            errorLogger.CommandText += "'error at FindAllPostEventClaims " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return retList;
        }

        public IList<int> GetOrdersByDomain(string searchTerm)
        {
            IList<int> orderIds = new List<int>();
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var GetOrdersByDomain = new SqlCommand("GetOrdersByDomain", sqlConnection))
                {
                    try
                    {
                        GetOrdersByDomain.Connection = sqlConnection;
                        GetOrdersByDomain.CommandType = CommandType.StoredProcedure;
                        var searchParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@searchTerm",
                            Value = searchTerm
                        };
                        GetOrdersByDomain.Parameters.Add(searchParam);

                        using (var reader = GetOrdersByDomain.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                var orderId = reader.GetInt32(0);
                                orderIds.Add(orderId);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindAllPostEventClaims' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindAllPostEventClaims', 9 ,";
                            errorLogger.CommandText += "'error at FindAllPostEventClaims " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return orderIds;
        }

        public string CreateOnDemandClaimForMigratedOrder(OrderRow row, DateTime getExpiry)
        {
            return "";
        }

        public string GetOnDemandClaimById(int idOrder)
        {

            string retClaim = "";
            var i = -1;
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var GetOnDemandClaim = new SqlCommand("GetOnDemandClaimById", sqlConnection))
                {
                    try
                    {
                        GetOnDemandClaim.Connection = sqlConnection;
                        GetOnDemandClaim.CommandType = CommandType.StoredProcedure;
                        var onDemandCodeParameter = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@idOrder",
                            Value = idOrder
                        };
                        GetOnDemandClaim.Parameters.Add(onDemandCodeParameter);

                        using (var reader = GetOnDemandClaim.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                retClaim = reader[0].ToString();
                                i++;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindAllPostEventClaims' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindAllPostEventClaims', 9 ,";
                            errorLogger.CommandText += "'error at FindAllPostEventClaims " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            if (i == -1) retClaim = "none found";
            if (i > 0) retClaim = "duped";

            return retClaim;


        }

        public string GetOnDemandClaimByCode(string onDemandCode)
        {

            string retClaim = "";
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var GetOnDemandClaimByCode = new SqlCommand("GetOnDemandClaimByCode", sqlConnection))
                {
                    try
                    {
                        GetOnDemandClaimByCode.Connection = sqlConnection;
                        GetOnDemandClaimByCode.CommandType = CommandType.StoredProcedure;
                        var onDemandCodeParameter = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@onDemandCode",
                            Value = onDemandCode
                        };
                        GetOnDemandClaimByCode.Parameters.Add(onDemandCodeParameter);
                        retClaim = GetOnDemandClaimByCode.ExecuteScalar().ToString();


                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FindAllPostEventClaims' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FindAllPostEventClaims', 9 ,";
                            errorLogger.CommandText += "'error at FindAllPostEventClaims " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return retClaim;


        }

        public List<int> FindAllPossibleRegTypesByWebinarId(int id)
        {
            List<int> regTypeIds = new List<int>();

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                //--EXEC  @idWebinar = 2016, @label = 'Live Plus Five', @findAllPossible = null
                using (var myConn = new SqlCommand("GetAllPossibleRegTypesPerWebinar", sqlConnection))
                {
                    try
                    {
                        myConn.Connection = sqlConnection;
                        myConn.CommandType = CommandType.StoredProcedure;
                        var idWebinar = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@idWebinar",
                            Value = id
                        };
                        myConn.Parameters.Add(idWebinar);

                        using (var reader = myConn.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var thisRegType = (Convert.ToInt32(reader[0]));
                                regTypeIds.Add(thisRegType);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'findAllPossible' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'findAllPossible', 9 ,";
                            errorLogger.CommandText += "'error at findAllPossible " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return regTypeIds;



        }

        public void UpdateUserEmail(string oldEmail, string email)
        {
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var UpdateUserEmail = new SqlCommand("UpdateUserEmail", sqlConnection))
                {
                    try
                    {
                        UpdateUserEmail.Connection = sqlConnection;
                        UpdateUserEmail.CommandType = CommandType.StoredProcedure;
                        var updateUserEmailNew = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@newEmail",
                            Value = email
                        };
                        UpdateUserEmail.Parameters.Add(updateUserEmailNew);

                        var updateUserEmailOld = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@oldEmail",
                            Value = oldEmail
                        };
                        UpdateUserEmail.Parameters.Add(updateUserEmailOld);

                        UpdateUserEmail.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'UpdateUserEmail' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'UpdateUserEmail', 9 ,";
                            errorLogger.CommandText += "'error at UpdateUserEmail " + ex.Message.Replace("'", "|") +
                                                       "')";

                            errorLogger.ExecuteNonQuery();

                        }
                        throw;
                    }
                }
            }
        }

        public string InsertOnDemandClaim(int orderId)
        {
            var result = "failed on" + orderId;
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var insertOnDemandClaim = new SqlCommand("InsertOnDemandClaim", sqlConnection))
                {
                    try
                    {
                        insertOnDemandClaim.Connection = sqlConnection;
                        insertOnDemandClaim.CommandType = CommandType.StoredProcedure;
                        var updateUserEmailNew = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@idOrder",
                            Value = orderId
                        };
                        insertOnDemandClaim.Parameters.Add(updateUserEmailNew);

                        result = insertOnDemandClaim.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'InsertOnDemandClaim' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'InsertOnDemandClaim', 9 ,";
                            errorLogger.CommandText += "'error at InsertOnDemandClaim " + ex.Message.Replace("'", "|") +
                                                       "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return result;
        }

        public int CheckIfEmailAlreadyRegisteredForWebinar(string orderEmail, int webinarId)
        {
            var result = "";
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (var checkIfOrderExists
                    = new SqlCommand("CheckIfEmailAlreadyRegisteredForWebinar", sqlConnection))
                {
                    try
                    {
                        checkIfOrderExists.Connection = sqlConnection;
                        checkIfOrderExists.CommandType = CommandType.StoredProcedure;

                        var newEmailParm = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@email",
                            Value = orderEmail
                        };
                        checkIfOrderExists.Parameters.Add(newEmailParm);

                        var idWebinarParm = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@idWebinar",
                            Value = webinarId
                        };
                        checkIfOrderExists.Parameters.Add(idWebinarParm);


                        result = checkIfOrderExists.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CheckIfEmailAlreadyRegisteredForWebinar' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CheckIfEmailAlreadyRegisteredForWebinar', 9 ,";
                            errorLogger.CommandText += "'error at CheckIfEmailAlreadyRegisteredForWebinar " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
                return Convert.ToInt32(result);
            }
        }

        public string GetPreSaveValues(int idOrder)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getPricingsCommand = new SqlCommand())
                {
                    var orderIdParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@idOrder",
                        Value = idOrder
                    };

                    getPricingsCommand.Connection = sqlConnection;
                    getPricingsCommand.CommandType = CommandType.Text;
                    getPricingsCommand.Parameters.Add(orderIdParameter);
                    getPricingsCommand.CommandText =
                        "SELECT r.RowPrice" +
                        ", o.OrderStatus " +
                        ", ISNULL(r.Discount_idDiscount, 0) " +
                        ", ISNULL((SELECT Sum(Price) FROM dbo.AdditionalLocation WHERE idOrderRow = r.idOrderRow), 0) " +
                        ", (SELECT RegTypeLabel FROM dbo.RegType WHERE idRegType =  r.idRegType)" +
                        ", r.idRegType" +
                        " FROM	dbo.[Order] o INNER JOIN dbo.OrderRow r ON r.idOrder = o.idOrder WHERE o.idOrder = @idOrder;";
                    string getPreSaveValues = "";

                    using (var reader = getPricingsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            getPreSaveValues = reader[0] + "," + reader[1] + "," + reader[2] + "," + reader[3] + "," + reader[4] + ", " + reader[5];
                        }
                    }

                    return getPreSaveValues;
                }
            }
        }

        public string CheckForAnyOrders(DateTime startDate, int idAffiliate)
        {
            SqlDataReader reader;

            var endDate = startDate.AddDays(7);
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var checkForAnyOrders = new SqlCommand("GetOrdersForWeeklyInvoiceByAffiliate", sqlConnection))
                {
                    try
                    {
                        checkForAnyOrders.Connection = sqlConnection;
                        checkForAnyOrders.CommandType = CommandType.StoredProcedure;

                        var startDateParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@StartDate",
                            Value = startDate
                        };
                        checkForAnyOrders.Parameters.Add(startDateParam);

                        var idAffiliateParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@idAffiliate",
                            Value = idAffiliate
                        };

                        checkForAnyOrders.Parameters.Add(idAffiliateParam);
                        reader = checkForAnyOrders.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CheckForAnyOrders' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CheckForAnyOrders', 9 ,";
                            errorLogger.CommandText += "'error at CheckForAnyOrders " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }


                StringBuilder sb = new StringBuilder();
                StringBuilder sbWO = new StringBuilder();
                StringBuilder sbPEO = new StringBuilder();
                StringBuilder sbADJ = new StringBuilder();
                sb.Append(idAffiliate + Environment.NewLine);
                if (reader.HasRows)
                {
                    while (reader.HasRows)
                    {

                        //an interesting bug in how the reader is behaving here:
                        //  the sproc being called returns 3 resultsets. If any one of
                        //  those resultsets contains no members, none of the remaining
                        //  resultsets will have any data in the reader.
                        //E.G. if 'WebinarsOrders' is blank neither PostEvent nor Adjusted
                        //  will return the values that the sproc output.

                        while (reader.Read())
                        {
                            if (reader.GetName(0) == "WebinarOrders")
                            {
                                sbWO.Append(reader.GetInt32(0) + ",");
                            }

                            if (reader.GetName(0) == "PostEventOrders")
                            {
                                sbPEO.Append(reader.GetInt32(0) + ",");
                            }

                            if (reader.GetName(0) == "AjustedOrders")
                            {
                                sbADJ.Append(reader.GetInt32(0) + ",");
                            }
                        }

                        reader.NextResult();
                    }
                }
                else
                {
                    return "none found: " + idAffiliate;
                }

                return sb.ToString() + " WebinarOrders: " + sbWO.ToString().TrimEnd(',') + " PostEventOrders: " + sbPEO + " AjustedOrders: " + sbADJ;
            }

        }

        public int CreateAdjustmentDiscount(string note, string amtToDiscount, int orderId)
        {
            var result = 0;

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var createDiscount = new SqlCommand("CreateDiscountCode", sqlConnection))
                {
                    try
                    {
                        createDiscount.Connection = sqlConnection;
                        createDiscount.CommandType = CommandType.StoredProcedure;


                        var NotesParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@Notes",
                            Value = note
                        };

                        var AmountOfDiscountParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@AmountOfDiscount",
                            Value = amtToDiscount
                        };

                        var TypeOfDiscountParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Decimal,
                            ParameterName = "@TypeOfDiscount",
                            Value = 3
                        };


                        //var TotalCountParam = new SqlParameter
                        //{
                        //    SqlDbType = SqlDbType.Decimal,
                        //    ParameterName = "@TotalCount",
                        //    Value = 0
                        //};


                        var DiscountCodeParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@DiscountCode",
                            Value = "adj" + orderId
                        };

                        //createDiscount.Parameters.Add(TotalCountParam);
                        createDiscount.Parameters.Add(DiscountCodeParam);
                        createDiscount.Parameters.Add(TypeOfDiscountParam);
                        createDiscount.Parameters.Add(AmountOfDiscountParam);
                        createDiscount.Parameters.Add(NotesParam);

                        result = (int)createDiscount.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {

                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CreateAdjustmentDiscount' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CreateAdjustmentDiscount', 9 ,";
                            errorLogger.CommandText += "'error at CreateAdjustmentDiscount " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
                return result;

            }
        }

        public object UpdateAffiliate(Affiliate affiliate)
        {
            var result = 0;

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var updateAff = new SqlCommand("UpdateAffiliate", sqlConnection))
                {
                    try
                    {
                        updateAff.Connection = sqlConnection;
                        updateAff.CommandType = CommandType.StoredProcedure;


                        var IDAffParam = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idAff", Value = affiliate.idUserAff };
                        var WebFooterParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@WebFooter", Value = affiliate.WebFooter };
                        var Logo = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@Logo", Value = affiliate.Logo };
                        var EmailBannerParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailBanner", Value = affiliate.EmailBanner };
                        var EmailFooterParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@EmailFooter", Value = affiliate.EmailFooter };
                        var BillingModelParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@BillingModel", Value = affiliate.BillingModel };
                        var CommissionModelParam = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@CommissionModel", Value = affiliate.CommissionModel };
                        var NotiOrdersParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@NotiOrders", Value = affiliate.NotiOrders };
                        var NotiPromosParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@NotiPromos", Value = affiliate.NotiPromos };
                        var MailChimpListParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@idMailChimpList", Value = affiliate.idMailChimpList };
                        var NotiInvoicesParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@NotiInvoices", Value = affiliate.NotiInvoices };
                        var TimeZoneParam = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@TimeZone", Value = (int)affiliate.WebUser.timeZone };
                        //
                        var SenderEmailParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@PromoSenderEmail", Value = affiliate.PromoSenderEmail };
                        var SenderNameParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@PromoSenderName", Value = affiliate.PromoSenderName };


                        //createDiscount.Parameters.Add(TotalCountParam);
                        updateAff.Parameters.Add(IDAffParam);
                        updateAff.Parameters.Add(WebFooterParam);
                        updateAff.Parameters.Add(Logo);
                        updateAff.Parameters.Add(EmailBannerParam);
                        updateAff.Parameters.Add(EmailFooterParam);
                        updateAff.Parameters.Add(BillingModelParam);
                        updateAff.Parameters.Add(CommissionModelParam);
                        updateAff.Parameters.Add(MailChimpListParam);
                        updateAff.Parameters.Add(NotiInvoicesParam);
                        updateAff.Parameters.Add(NotiOrdersParam);
                        updateAff.Parameters.Add(NotiPromosParam);
                        updateAff.Parameters.Add(TimeZoneParam);
                        updateAff.Parameters.Add(SenderNameParam);
                        updateAff.Parameters.Add(SenderEmailParam);

                        result = (int)updateAff.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {

                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CreateAdjustmentDiscount' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CreateAdjustmentDiscount', 9 ,";
                            errorLogger.CommandText += "'error at CreateAdjustmentDiscount " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
                return result;

            }
        }

        public int UpdatePresenter(Presenter newPresenter)
        {

            var result = 0;

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var updatePresenter = new SqlCommand("UpdatePresenter", sqlConnection))
                {
                    try
                    {
                        updatePresenter.Connection = sqlConnection;
                        updatePresenter.CommandType = CommandType.StoredProcedure;

                        var IDUserParam = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idUser", Value = newPresenter.idUser };
                        var BioParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@Biography", Value = newPresenter.Biography };
                        var BioLongParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@BiographyLong", Value = newPresenter.BiographyLong };
                        var PhotoFullParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@PhotoFull", Value = newPresenter.PhotoFull };
                        var PhotoThumbParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@PhotoThumb", Value = newPresenter.PhotoThumb };
                        var EmailParam = new SqlParameter { SqlDbType = SqlDbType.VarChar, ParameterName = "@Email", Value = newPresenter.Email };

                        updatePresenter.Parameters.Add(IDUserParam);
                        updatePresenter.Parameters.Add(BioParam);
                        updatePresenter.Parameters.Add(BioLongParam);
                        updatePresenter.Parameters.Add(PhotoFullParam);
                        updatePresenter.Parameters.Add(PhotoThumbParam);

                        updatePresenter.Parameters.Add(EmailParam);
                        result = (int)updatePresenter.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {

                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'UpdatePresenter' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'UpdatePresenter', 9 ,";
                            errorLogger.CommandText += "'error at UpdatePresenter " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
                return result;

            }

        }

        public string CreateCompliancePerspectivesSubscription(OrderRow row)
        {
            var result = "";

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var createCpCode = new SqlCommand("CreateCPCode", sqlConnection))
                {
                    try
                    {
                        createCpCode.Connection = sqlConnection;
                        createCpCode.CommandType = CommandType.StoredProcedure;

                        var idOrder = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idOrder", Value = row.idOrder };
                        var StartDate = new SqlParameter { SqlDbType = SqlDbType.DateTime, ParameterName = "@StartDate", Value = row.Webinar.Date };

                        createCpCode.Parameters.Add(idOrder);
                        createCpCode.Parameters.Add(StartDate);

                        result = createCpCode.ExecuteScalar().ToString();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {

                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CreateCPCode' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CreateCPCode', 9 ,";
                            errorLogger.CommandText += "'error at CreateCPCode " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
                return result.ToString();

            }
        }

        public void insertRegTypeId(int webinarIdWebinar)
        {
            var result = "";
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var insertRegType = new SqlCommand("insertDESRegType", sqlConnection))
                {
                    try
                    {
                        insertRegType.Connection = sqlConnection;
                        insertRegType.CommandType = CommandType.StoredProcedure;

                        var idWebinar = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idWebinar", Value = webinarIdWebinar };


                        insertRegType.Parameters.Add(idWebinar);


                        insertRegType.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {

                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'CreateCPCode' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'CreateCPCode', 9 ,";
                            errorLogger.CommandText += "'error at CreateCPCode " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }
            }
        }
        
        public bool WebinarIsPast(int id)
        {
            bool webinarIsPast = false;
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var myConn = new SqlCommand("WebinarIsPast", sqlConnection))
                {
                    try
                    {
                        myConn.Connection = sqlConnection;
                        myConn.CommandType = CommandType.StoredProcedure;
                        var idWebinar = new SqlParameter
                        {
                            SqlDbType = SqlDbType.Int,
                            ParameterName = "@id",
                            Value = id
                        };
                        myConn.Parameters.Add(idWebinar);
                        var message = myConn.ExecuteScalar().ToString();

                        if (message == "true")
                        {
                            webinarIsPast = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'WebinarIsPast' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'WebinarIsPast', 9 ,";
                            errorLogger.CommandText += "'error at WebinarIsPast " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            
            return webinarIsPast;
        }

        public string User_HealThySelf(string email, string tenant)
        {
            var reply = "";

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (var myConn = new SqlCommand("FixMissingUserAccount", sqlConnection))
                {
                    try
                    {
                        myConn.Connection = sqlConnection;
                        myConn.CommandType = CommandType.StoredProcedure;
                        var emailParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@email",
                            Value = email
                        };
                        myConn.Parameters.Add(emailParam);

                        var tenantParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@tenant",
                            Value = tenant
                        };
                        myConn.Parameters.Add(tenantParam);




                        using (var reader = myConn.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reply = reader[0].ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'FixMissingUserAccount' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'FixMissingUserAccount', 9 ,";
                            errorLogger.CommandText += "'error at FixMissingUserAccount " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
            return reply;
        }
    }
}
