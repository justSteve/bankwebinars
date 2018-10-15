using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
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

        /// <summary>
        ///    Ensure OnDemand and Join codes are unique.
        /// </summary>
        /// <sql>CheckForUnique</sql>    
        /// <serves>
        /// All
        /// </serves>

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
                        LogError("CheckForUnique", "CheckForUnique: " + CheckForUnique.CommandText +
                                                      " Exception.Message: " + ex.Message);
                    }
                    return returnVal;

                }
            }
        }

        /// <summary>
        ///    Ensure MailChimp's id is unique.
        /// </summary>
        /// <sql>CheckForUnique</sql>    
        /// <serves>
        /// All
        /// </serves>

        public string CheckForUniqueLinkId(string code)
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

                    CheckForUnique.Parameters.Add(codeParameter);

                    CheckForUnique.Connection = sqlConnection;
                    CheckForUnique.CommandType = CommandType.StoredProcedure;
                    CheckForUnique.CommandText = "CheckForUniqueLinkId";

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
                        LogError("CheckForUniqueLinkId", "CheckForUniqueLinkId: " + CheckForUnique.CommandText +
                                                      " Exception.Message: " + ex.Message);
                    }
                    return returnVal;

                }
            }
        }

        /// <summary>
        ///    RateWatch importer's RegType provider.
        /// </summary>
        /// <sql>text</sql>    
        /// <serves>
        /// All
        /// </serves>
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
                    getLegacyWebinars.CommandText = "SELECT ttsLable from RateWatchImporter where rwLable = '" + regTypeLable +
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
                        LogError("FindRegTypeForRateWatch", "Unknown Registration Type: " + getLegacyWebinars.CommandText +
                                                      " Exception.Message: " + ex.Message);
                    }
                    return returnLable;

                }
            }
        }

        /// <summary>
        ///    ACS importer's RegType provider.
        /// </summary>
        /// <sql>text</sql>    
        /// <serves>
        /// All
        /// </serves>
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


        /// <summary>
        /// todo: refactor to populate all regtype values as AppVars.
        /// </summary>
        /// <sql>text</sql>    
        /// <param name="idRegType"></param>
        /// <returns></returns>
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
        // todo: refactor to populate all regtype values as AppVars.
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

        /// <summary>
        /// This method provides timezone based on users' zipcode.
        /// Hits our very old yet servicable 'timezone/zipcode' db. 
        /// There are more accurate alternatives -- should we migrate?
        /// </summary>
        /// <param name="idWebinar"></param>
        /// <param name="registrationType">Friendly Lable</param>
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



        //depricated but should consider replicating since this serves the 
        // self healing feature
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

        ////
        ///// <summary>
        ///// seems depricated 
        ///// </summary>
        ///// <sql>text</sql>    
        ///// <param name="idRegType"></param>
        ///// <returns></returns>
        //public Double[] GetCostOfUpgrades(int idRegType)
        //{
        //    using (var sqlConnection = new SqlConnection(_connectionString))
        //    {
        //        sqlConnection.Open();

        //        using (var getPricingsCommand = new SqlCommand())
        //        {
        //            var webinarIdParameter = new SqlParameter
        //            {
        //                SqlDbType = SqlDbType.Int,
        //                ParameterName = "@idRegType",
        //                Value = idRegType
        //            };


        //            getPricingsCommand.Connection = sqlConnection;
        //            getPricingsCommand.CommandType = CommandType.Text;
        //            getPricingsCommand.Parameters.Add(webinarIdParameter);
        //            getPricingsCommand.CommandText =
        //                "SELECT [base],[plus6],[premier] FROM [dbo].[UpgradePricing] WHERE  idRegType = @idRegType";

        //            var aryReturn = new Double[3];

        //            using (var reader = getPricingsCommand.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    aryReturn[0] = reader.GetDouble(0);
        //                    aryReturn[1] = reader.GetDouble(1);
        //                    aryReturn[2] = reader.GetDouble(2);

        //                }
        //            }
        //            return aryReturn;
        //        }
        //    }
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// adapt this for use by new auth system?
        /// </summary>
        /// <param name="tenant"></param>
        /// <sql>text</sql>    
        /// <param name="key"></param>
        /// <returns></returns>
        // 
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

        /// <summary>
        /// housekeeping - cleans up possible remants of impersonated sessions.
        /// </summary>
        /// <sql>text</sql>    
        /// <param name="adminUserEmail"></param>
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

        //test for breakage
        //private static OrderStatus SetOrderStatus(int mkStatus)
        //{
        //    OrderStatus setStatus = OrderStatus.Abandoned;
        //    // Error = 0, 
        //    //InProcess = 1,
        //    //Submitted = 2,
        //    //Billed = 3,
        //    //Paid = 4,
        //    //Abandoned = 5,
        //    //Canceled = 6,
        //    //AwaitingVerification = 7,
        //    //Unknown = 255
        //    switch (mkStatus)
        //    {
        //        case 1:
        //            setStatus = OrderStatus.InProcess;
        //            break;
        //        case 0:
        //            setStatus = OrderStatus.Error;
        //            break;
        //        case 2:
        //            setStatus = OrderStatus.Submitted;
        //            break;
        //        case 3:
        //            setStatus = OrderStatus.Billed;
        //            break;
        //        case 4:
        //            setStatus = OrderStatus.Paid;
        //            break;
        //        case 5:
        //            setStatus = OrderStatus.Abandoned;
        //            break;
        //        case 6:
        //            setStatus = OrderStatus.Canceled;
        //            break;
        //        case 7:
        //            setStatus = OrderStatus.AwaitingVerification;
        //            break;
        //    }
        //    return setStatus;
        //}

        //in current usage
        // serves all tenants

        /// <summary>
        /// FindPostEventClaimByOnDemandCode to build OD link
        /// </summary>
        /// <param name="order"></param>
        /// <returns>PostEventClaim as string</returns>
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


        /// <summary>
        ///  in current usage
        ///  serves all tenants
        ///  could probably be replaced by EF Lambda.
        /// FindPostEventClaimByOrderId
        /// </summary>
        /// <param name="order"></param>
        /// <returns>PostEventClaim</returns>
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



        /// <summary>
        ///  in current usage
        ///  serves all tenants
        ///  could probably be replaced by EF Lambda.
        /// FindAllPostEventClaims
        /// </summary>
        /// <param name="order"></param>
        /// <returns>List of PostEventClaim</returns>
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
                        PostEventClaim thisClaim;

                        using (var reader = FindAllPostEventClaims.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //PostEventClaim returnClaim = new PostEventClaim();
                                thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(reader[0].ToString());
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




        /// <summary>
        ///  in current usage
        ///  serves all tenants
        ///  could probably be replaced by EF Lambda.
        /// GetOrdersByDomain
        /// </summary>
        /// <param name="order"></param>
        /// <returns>PostEventClaim</returns>
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

        //public string CreateOnDemandClaimForMigratedOrder(OrderRow row, DateTime getExpiry)
        //{
        //    return "";
        //}


        /// <summary>
        ///    Provides OD code to BuildRecordingIsPostedMessage template 
        /// </summary>
        ///     GetOnDemandClaimById
        /// <serves>
        /// All
        /// </serves>
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

        /// <summary>
        ///    Provides code to OD Playback page
        /// </summary>
        ///     GetOnDemandClaimByCode
        /// <serves>
        /// All
        /// </serves>
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


        // 
        /// <summary>
        /// todo: refactor to populate all regtype values as AppVars.
        /// GetAllPossibleRegTypesPerWebinar
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// HACK: stepping carefully around the API way of doing things
        /// I've been unable to resovle the error tossed when updating email by the book:
        ///  {"EmailIsUsername is enabled in SecuritySettings -- use ChangeEmail APIs instead."}
        ///  https://gitter.im/brockallen/BrockAllen.MembershipReboot/archives/2015/10/22
        /// </summary>
        ///     UpdateUserEmail
        /// <serves>
        /// All
        /// </serves>
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


        /// <summary>
        ///    Provides OD code to BuildRecordingIsPostedMessage template 
        /// </summary>
        ///     InsertOnDemandClaim
        /// <serves>
        /// All
        /// </serves>
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


        /// <summary>
        ///    Prevents duplicate orders per user
        /// </summary>
        ///     CheckIfEmailAlreadyRegisteredForWebinar
        /// <serves>
        /// All
        /// </serves>
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
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
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

        /// <summary>
        ///    Prevents duplicate orders per domain
        /// </summary>
        ///     CheckIfEmailAlreadyRegisteredForWebinarByDomain
        /// <serves>
        /// All
        /// </serves>
        public int CheckIfEmailAlreadyRegisteredForWebinarByDomain(string orderEmail, int webinarId)
        {
            var result = "";
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (var checkIfOrderExists
                    = new SqlCommand("CheckIfEmailAlreadyRegisteredForWebinarByDomain", sqlConnection))
                {
                    try
                    {
                        checkIfOrderExists.Connection = sqlConnection;
                        checkIfOrderExists.CommandType = CommandType.StoredProcedure;

                        var newEmailParm = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@email",
                            Value = orderEmail.Split('@')[1]
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
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
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


        /// <summary>
        ///    When existing order is edited this method provides original values
        /// </summary>
        /// <sql>text</sql>    
        /// <serves>
        /// All
        /// </serves>
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
                        ", o.Total" +
                        " FROM	dbo.[Order] o INNER JOIN dbo.OrderRow r ON r.idOrder = o.idOrder WHERE o.idOrder = @idOrder;";
                    string getPreSaveValues = "";

                    using (var reader = getPricingsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            getPreSaveValues = reader[0] + "," + reader[1] + "," + reader[2] + "," + reader[3] + "," + reader[4] + ", " + reader[5] + ", " + reader[6];
                        }
                    }

                    return getPreSaveValues;
                }
            }
        }

        /// <summary>
        ///    Generate Weekly Invoices
        /// </summary>
        ///     GetOrdersForWeeklyInvoiceByAffiliate
        /// <serves>
        /// All
        /// </serves>
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
                StringBuilder sbInc = new StringBuilder();
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
                            if (reader.GetName(0) == "IncompleteOrders")
                            {
                                sbInc.Append(reader.GetInt32(0) + ",");
                            }
                        }

                        reader.NextResult();
                    }
                }
                else
                {
                    return "none found: " + idAffiliate;
                }

                return sb.ToString() + " WebinarOrders: " + sbWO.ToString().TrimEnd(',') + " PostEventOrders: " + sbPEO + " AjustedOrders: " + sbADJ + " IncompleteOrders: " + sbInc;
            }

        }


        /// <summary>
        ///    Supports manual price adjustment
        /// </summary>
        ///     CreateDiscountCode
        /// <serves>
        /// All
        /// </serves>
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


        /// <summary>
        ///    Update Affiliate's fields
        /// </summary>
        ///     UpdateAffiliate
        /// <serves>
        /// All
        /// </serves>
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



        /// <summary>
        ///    Update Presenter's fields
        /// </summary>
        ///     UpdateAffiliate
        /// <serves>
        /// All
        /// </serves>
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



        //Depricated
        /// <summary>
        ///    Was only used when LearnUpon was initialized
        /// </summary>
        ///     insertDESRegType
        /// <serves>
        ///     DES
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

        //used by FindRegTypesAvailableToExistingOrder
        /// <summary>
        ///    Determines which registration types to display
        /// </summary>
        ///     WebinarIsPast
        /// <serves>
        /// All
        /// </serves>
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



        /// <summary>
        ///    Runs when error condition is triggered by missing user record
        /// </summary>
        ///     FixMissingUserAccount
        /// <serves>
        /// All
        /// </serves>
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




        /// <summary>
        ///    Moves webinar's status to 'recorded'.
        /// </summary>
        ///     updateRegGroups
        /// <serves>
        ///     varies by tenant
        public void WebinarIsSetToRecorded()
        {
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.StoredProcedure;
                    getLegacyWebinars.CommandText = "updateRegGroups";

                    try
                    {
                        int idWebinar;
                        DateTime date;
                        using (var sqlUpdateConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
                        {
                            sqlUpdateConnection.Open();

                            using (var reader = getLegacyWebinars.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    //reader.GetInt32(0), reader.GetDecimal(1)));
                                    idWebinar = reader.GetInt32(0);
                                    //date = reader.GetDateTime(1);
                                    //var timeToStartWebinar = date.AddMinutes(-30) - DateTime.Now;
                                    //AddMessage(idWebinar.ToString(), timeToStartWebinar);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
        }


        //TODO: Refactor to account for malformed AuditNotes
        /// <summary>
        ///    Provides audit notes to admins
        /// </summary>
        ///     GetAuditNotes
        /// <serves>
        /// All
        /// </serves>
        public string GetAuditNotes(Order order, string auditType)
        {
            List<AuditChangedRegTypeModel> result = new List<AuditChangedRegTypeModel>();
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {

                sqlConnection.Open();
                using (var myConn = new SqlCommand("GetAuditNotes", sqlConnection))
                {
                    try
                    {
                        myConn.Connection = sqlConnection;
                        myConn.CommandType = CommandType.StoredProcedure;
                        var idOrderParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@idOrder",
                            Value = order.idOrder
                        };


                        var auditTypeParam = new SqlParameter
                        {
                            SqlDbType = SqlDbType.VarChar,
                            ParameterName = "@auditType",
                            Value = auditType
                        };
                        myConn.Parameters.Add(idOrderParam);
                        myConn.Parameters.Add(auditTypeParam);

                        using (var reader = myConn.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader[0] != null)
                                {
                                    var _reply = new AuditChangedRegTypeModel();
                                    _reply.DateOfChange = reader[0].ToString();
                                    var startBlock = reader[1].ToString().IndexOf("from: ") + 6;
                                    var endBlock = reader[1].ToString().IndexOf("to: ");

                                    _reply.From = reader[1].ToString().Substring(startBlock, endBlock - startBlock);

                                    if (reader[1].ToString().Contains("note"))
                                    {

                                        startBlock = reader[1].ToString().IndexOf("to: ") + 4;
                                        endBlock = reader[1].ToString().IndexOf("note:");
                                        _reply.To = reader[1].ToString().Substring(startBlock, endBlock - startBlock);

                                        startBlock = reader[1].ToString().IndexOf("note: ") + 6;
                                        endBlock = reader[1].ToString().IndexOf("by: ");
                                        _reply.Note = reader[1].ToString().Substring(startBlock, endBlock - startBlock);
                                    }
                                    else
                                    {
                                        startBlock = reader[1].ToString().IndexOf("to: ") + 4;
                                        endBlock = reader[1].ToString().IndexOf("by:");
                                        _reply.To = reader[1].ToString().Substring(startBlock, endBlock - startBlock);
                                        _reply.Note = "";
                                    }
                                    startBlock = reader[1].ToString().IndexOf("\"RemoteUser\": ") + 14;
                                    endBlock = reader[1].ToString().IndexOf("\"UserAgent\":");
                                    _reply.By = reader[1].ToString().Substring(startBlock, endBlock - startBlock)
                                        .Replace("\",\r\n  ", "")
                                        .Replace("\"", "");

                                    result.Add(_reply);
                                }
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
                            errorLogger.CommandText += "'GetAuditNotes' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'GetAuditNotes', 9 ,";
                            errorLogger.CommandText += "'error at GetAuditNotes " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                        return null;
                    }
                }
            }
            var returnResult = JsonConvert.SerializeObject(result);
            return returnResult;
        }

        /// <summary>
        ///    Not currently used
        /// </summary>
        ///     BuildStampsLabel
        /// <serves>
        /// All
        /// </serves>
        public void BuildStampsLabels(int orderIdOrder)
        {

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();

                using (var buildStampsLabel = new SqlCommand())
                {
                    var idOrderParam = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        ParameterName = "@idOrder",
                        Value = orderIdOrder
                    };


                    buildStampsLabel.Connection = sqlConnection;
                    buildStampsLabel.CommandType = CommandType.StoredProcedure;
                    buildStampsLabel.CommandText = "BuildStampsLabel";

                    buildStampsLabel.Parameters.Add(idOrderParam);

                    try
                    {
                        buildStampsLabel.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {

                        using (
                            var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts + "',";
                            errorLogger.CommandText += "'BuildStampsLabels' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'BuildStampsLabels', 9 ,";
                            errorLogger.CommandText += "'error at BuildStampsLabels " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }
        }

        /// <summary>
        ///    Housekeeping - deletes GhostInspector results
        /// </summary>
        ///     sql
        /// <serves>
        /// All
        /// </serves>
        public void GI_DeleteExistingUser()
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "Delete from WebUser where email = 'giNew@existing.com'";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            getLegacyWebinars.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("GI_DeleteExistingEmail", "GI_DeleteExistingEmail: " + getLegacyWebinars.CommandText +
                                                      " Exception.Message: " + ex.Message);
                    }

                }
            }
        }


        /// <summary>
        ///    Housekeeping - deletes GhostInspector results
        /// </summary>
        ///     sql
        /// <serves>
        /// All
        /// </serves>
        public void GI_DeleteExistingDomain()
        {
            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "Delete from WebUser where email like '%@newExisting.com'";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            getLegacyWebinars.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("GI_DeleteExistingDomain", "GI_DeleteExistingDomain: " + getLegacyWebinars.CommandText +
                                                           " Exception.Message: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        ///    Housekeeping - GhostInspector 
        /// </summary>
        ///     sql
        /// <serves>
        /// All
        /// </serves>
        public void GI_DeleteExistingOrder()
        {

            var returnLable = "";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "DELETE FROM dbo.[Order] WHERE email = 'giOld@existing.com'";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            getLegacyWebinars.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("GI_DeleteExistingOrder", "GI_DeleteExistingOrder: " + getLegacyWebinars.CommandText +
                                                            " Exception.Message: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        ///    Housekeeping - GhostInspector 
        /// </summary>
        ///     sql
        /// <serves>
        /// All
        /// </serves>
        public void GI_SetExistingOrderToInProcess()
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
                        "UPDATE dbo.[Order] SET OrderStatus = 1 WHERE BillingEmail = 'giOld@existing.com'";

                    try
                    {
                        using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                        {
                            sqlUpdateConnection.Open();
                            getLegacyWebinars.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("GI_SetExistingOrderToInProcess", "GI_SetExistingOrderToInProcess: " +
                                                                   getLegacyWebinars.CommandText +
                                                                   " Exception.Message: " + ex.Message);
                    }
                }
            }
        }

        public List<MigrateMCUsers> UsersFromSheet()
        {

            SqlDataReader reader;

            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                using (
                    var usersFromSheet = new SqlCommand())
                {
                    try
                    {

                        usersFromSheet.Connection = sqlConnection;
                        usersFromSheet.CommandType = CommandType.Text;

                        usersFromSheet.CommandText = "SELECT * FROM [dbo].[importSheet] ";

                        reader = usersFromSheet.ExecuteReader();
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                            errorLogger.CommandText += "'usersFromSheet' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'usersFromSheet', 9 ,";
                            errorLogger.CommandText += "'error at usersFromSheet " +
                                                       ex.Message.Replace("'", "|") + "')";

                            errorLogger.ExecuteNonQuery();
                        }

                        throw;
                    }
                }


                StringBuilder sb = new StringBuilder();
                List<MigrateMCUsers> users = new List<MigrateMCUsers>();
                if (reader.HasRows)
                {
                    while (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            try
                            {

                                var user = new MigrateMCUsers
                                {
                                    Email_Address = reader.GetString(2),
                                    First_Name = reader.SafeGetString(0),
                                    Last_Name = reader.SafeGetString(1),
                                    Source = reader.SafeGetString(5),
                                    //Welcome_Sequence_Complete_ = reader.SafeGetString(4),
                                    Purchased = reader.SafeGetString(4),
                                    Mailing_List = reader.SafeGetString(3),
                                    Keep_Me_Informed_About_ = reader.SafeGetString(6)

                                };

                                users.Add(user);
                            }
                            catch (Exception e)
                            {

                                using (var errorLogger = new SqlCommand("logError", sqlConnection))
                                {
                                    errorLogger.CommandText =
                                        "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                                    errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortTimeString() + "',";
                                    errorLogger.CommandText += "'usersFromSheetReader' ,";
                                    errorLogger.CommandText += "9 ,9 ,9 ,'usersFromSheetReader', 9 ,";
                                    errorLogger.CommandText += "'error at usersFromSheetReader " +
                                                               e.Message.Replace("'", "|") + "')";

                                    errorLogger.ExecuteNonQuery();
                                }

                                throw;
                            }
                        }
                        reader.NextResult();
                    }
                }
                else
                {
                    return null;
                }

                return users;
            }

        }

        public void UpdateUserIdMailChimp(int ourUserIdUser, string ourUserIdMailChimp)
        {
            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var updateUserIdMailChimp = new SqlCommand())
                {
                    try
                    {

                        var idParam = new SqlParameter
                        {
                            DbType = DbType.Int32,
                            ParameterName = "@id",
                            Value = ourUserIdUser
                        };
                        var idMCParam = new SqlParameter
                        {
                            DbType = DbType.String,
                            ParameterName = "@idMC",
                            Value = ourUserIdMailChimp
                        };

                        updateUserIdMailChimp.Connection = sqlConnection;
                        updateUserIdMailChimp.CommandType = CommandType.Text;
                        updateUserIdMailChimp.Parameters.Add(idParam);
                        updateUserIdMailChimp.Parameters.Add(idMCParam);
                        updateUserIdMailChimp.CommandText =
                            "UPDATE dbo.WebUser SET idMailChimp = @idMC where idUser = @id";

                        numRows = updateUserIdMailChimp.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }


        }

        public string InsertViewTrackerClaim(string addToClaim, string userEmail)
        {
            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var insertViewTrackerClaim = new SqlCommand())
                {
                    try
                    {

                        var idParam = new SqlParameter
                        {
                            DbType = DbType.String,
                            ParameterName = "@value",
                            Value = addToClaim
                        };
                        var idMCParam = new SqlParameter
                        {
                            DbType = DbType.String,
                            ParameterName = "@email",
                            Value = userEmail
                        };

                        insertViewTrackerClaim.Connection = sqlConnection;
                        insertViewTrackerClaim.CommandType = CommandType.Text;
                        insertViewTrackerClaim.Parameters.Add(idParam);
                        insertViewTrackerClaim.Parameters.Add(idMCParam);
                        insertViewTrackerClaim.CommandText =
                            "INSERT dbo.UserClaims ( ParentKey, Type, Value ) VALUES " +
                            " (   (SELECT [Key] FROM dbo.UserAccounts " +
                            " WHERE Email = @email),   N'http://ttstrain.com/ws/2014/01/identity/claims/PostEventMaterialsWereAccessed', @value  )";

                        numRows = insertViewTrackerClaim.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        LogError("InsertViewTrackerClaim", "InsertViewTrackerClaim: " + insertViewTrackerClaim.CommandText +
                                                   " Exception.Message: " + e.Message);
                        return "error: " + e.Message;
                    }
                }
            }
            return numRows.ToString();
        }
        public string InsertMailChimpCampaign(MailChimp.Net.Models.Campaign campaign)
        {
            campaign.Links = null;
            campaign.VariateSettings = null;
            campaign.RssOptions = null;

            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var insertCampagin = new SqlCommand())
                {
                    try
                    {

                        var campaignParam = new SqlParameter
                        {
                            DbType = DbType.String,
                            ParameterName = "@json",
                            Value = JsonConvert.SerializeObject(campaign)
                        };

                        insertCampagin.Connection = sqlConnection;
                        insertCampagin.CommandType = CommandType.StoredProcedure;
                        insertCampagin.Parameters.Add(campaignParam);
                        insertCampagin.CommandText = "InsertCampaign";

                        numRows = insertCampagin.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        LogError("InsertMailChimpCampaign", "InsertMailChimpCampaign: " + insertCampagin.CommandText +
                                                   " Exception.Message: " + e.Message);
                        return "error: " + e.Message;
                    }
                }
            }
            return numRows.ToString();
        }

        public string InsertMailChimpUrlClicked(MailChimp.Net.Models.UrlClicked clickDetail)
        {
            

            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var insertCampagin = new SqlCommand())
                {
                    try
                    {
                        var campaignParam = new SqlParameter
                        {
                            DbType = DbType.String,
                            ParameterName = "@json",
                            Value = JsonConvert.SerializeObject(clickDetail)
                        };

                        insertCampagin.Connection = sqlConnection;
                        insertCampagin.CommandType = CommandType.StoredProcedure;
                        insertCampagin.Parameters.Add(campaignParam);
                        insertCampagin.CommandText = "InsertClicksDetails";

                        numRows = insertCampagin.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        LogError("InsertMailChimpCampaign", "InsertMailChimpCampaign: " + insertCampagin.CommandText +
                                                   " Exception.Message: " + e.Message);
                        return "error: " + e.Message;
                    }
                }
            }
            return numRows.ToString();
        }

        public string ConvertCfteaOrders()
        {
            int numRows = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var mySqlCmd = new SqlCommand())
                {
                    try
                    {
                        mySqlCmd.Connection = sqlConnection;
                        mySqlCmd.CommandType = CommandType.Text;
                        mySqlCmd.CommandText =
                            "UPDATE dbo.[Order] SET idAffiliate = 377 WHERE idAffiliate = 11464 AND  OrderStatus = 4 ";

                        numRows = mySqlCmd.ExecuteNonQuery();

                    }
                    catch (Exception e)
                    {
                        LogError("InsertViewTrackerClaim", "InsertViewTrackerClaim: " + mySqlCmd.CommandText +
                                                   " Exception.Message: " + e.Message);
                        return "error: " + e.Message;
                    }
                }
            }
            return numRows.ToString();
        }

    }

    public class MigrateMCUsers
    {
        public string Email_Address { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Source { get; set; }
        public string Welcome_Sequence_Complete_ { get; set; }
        public string Purchased { get; set; }
        public string Mailing_List { get; set; }
        public string Keep_Me_Informed_About_ { get; set; }
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
