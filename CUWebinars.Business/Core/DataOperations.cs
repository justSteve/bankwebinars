using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using CUWebinars.Business.Constants;

namespace CUWebinars.Business.Core
{
    public class DataOperations
    {
        private readonly string _connectionString;

        private readonly ILogger _logger;

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
                    getLegacyWebinars.CommandText = "SELECT ttsLable from ACSImporter where acsLable = '" + regTypeLable + "'";

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
                        _logger.Warn("Unknown Registration Type: " + getLegacyWebinars.CommandText + " Exception.Message: " + ex.Message);
                    }

                    return returnLable;

                }
            }
        }

        public IList<AdditionalLocationsPricing> GetAdditionalLocationsPricing(int webinarId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getPricingsCommand = new SqlCommand())
                {
                    var webinarIdParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@webinarId",
                        Value = webinarId
                    };

                    getPricingsCommand.Connection = sqlConnection;
                    getPricingsCommand.CommandType = CommandType.Text;
                    getPricingsCommand.Parameters.Add(webinarIdParameter);
                    getPricingsCommand.CommandText =
                        "SELECT id, cost FROM AdditionalLocationsLookupPrice WHERE idWebinar = @webinarId;";

                    IList<AdditionalLocationsPricing> pricingInformation = new List<AdditionalLocationsPricing>();

                    using (var reader = getPricingsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pricingInformation.Add(new AdditionalLocationsPricing { LookupPriceId = reader.GetInt32(0), Price = reader.GetDecimal(1) });
                        }
                    }

                    return pricingInformation;
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
                        _logger.Fatal("Invalid Registration Type {0} for idWebinar {1) ", registrationType, idWebinar);
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

        public string checkIfSteadfastIsOpen()
        {
            var isOpen = "";
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var checkIfOpen = new SqlCommand("select * from users where idUser = 19", sqlConnection))
                {
                    checkIfOpen.Connection = sqlConnection;
                    checkIfOpen.CommandType = CommandType.Text;
                    SqlDataReader reader = checkIfOpen.ExecuteReader();

                    while (reader.Read())
                    {
                        isOpen = "isOpen";
                    }
                    reader.Close();
                    return isOpen;

                }
            }
        }

        public
            string SendOrderToLegacy(Order newOrder)
        {
            var myRow = newOrder.OrderRows.FirstOrDefault();
            var addLoc = "";
            var addLocCount = 0;
            var orderid = "0";
            int translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);

            if (myRow.AdditionalLocation != null)
            {
                addLocCount = myRow.AdditionalLocation.Count;
                foreach (var loc in myRow.AdditionalLocation)
                {
                    addLoc += loc.Email + ",";
                }
            }

            var idUserParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idUser" };
            var idUser2Parameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idUser" };
            var idWebinarParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@idWebinar",
                Value = myRow.idWebinar
            };
            var idDiscountParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idDiscount" };
            var idOptionParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@idOption",
                Value = translatedOptionId
            };
            var addLocParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@addLoc",
                Value = addLoc
            };
            var addLocCountParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@addLocCount",
                Value = addLocCount
            };
            var firstNameParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@firstName",
                Value = newOrder.FirstName
            };
            var lastNameParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@lastName",
                Value = newOrder.LastName
            };
            var phone1Parameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@phone1",
                Value = newOrder.BillingPhone
            };
            var mAddressParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@mAddress",
                Value = newOrder.BillingAddress
            };
            var mCityParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@mCity",
                Value = newOrder.BillingCity
            };
            var mZipParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@mZip",
                Value = newOrder.BillingZip
            };
            var mStateParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@mState",
                Value = newOrder.BillingState
            };
            var emailParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@email",
                Value = newOrder.BillingEmail
            };
            var generalCommentsParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@generalComments",
                Value = "ImporterV2 " + newOrder.AffiliateComments
            };
            var provisionalInstitutionParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "@provisionalInstitution",
                Value = newOrder.Institution
            };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                int idUser = 0;
                sqlConnection.Open();
                using (var getUserID = new SqlCommand("InsertACSUser", sqlConnection))
                {
                    getUserID.Parameters.Add(firstNameParameter);
                    getUserID.Parameters.Add(lastNameParameter);
                    getUserID.Parameters.Add(phone1Parameter);
                    getUserID.Parameters.Add(mAddressParameter);
                    getUserID.Parameters.Add(mCityParameter);
                    getUserID.Parameters.Add(mZipParameter);
                    getUserID.Parameters.Add(mStateParameter);
                    getUserID.Parameters.Add(emailParameter);
                    getUserID.Parameters.Add(provisionalInstitutionParameter);
                    try
                    {
                        getUserID.Connection = sqlConnection;
                        getUserID.CommandType = CommandType.StoredProcedure;

                        idUser = Convert.ToInt32(getUserID.ExecuteScalar());
                        @idUserParameter.Value = idUser;
                        @idUser2Parameter.Value = idUser;
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() + "',";
                            errorLogger.CommandText += "'ACSIMPORTER' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[InsertACSUser]', 9 ,";
                            errorLogger.CommandText += "'error at InsertACSUser " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                    using (var errorLogger = new SqlCommand("logError", sqlConnection))
                    {
                        errorLogger.CommandText =
                            "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                        errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() + "',";
                        errorLogger.CommandText += "'ACSIMPORTER' ,";
                        errorLogger.CommandText += "0 ,0 ,0 ,'[InsertACSUser]', 0 ,";
                        errorLogger.CommandText += "'InsertACSUser returned " + idUser.ToString() + "')";

                        errorLogger.ExecuteNonQuery();
                    }
                }

                using (var sendOrder = new SqlCommand("ImportOrderACS", sqlConnection))
                {
                    sendOrder.Parameters.Add(idUser2Parameter);
                    sendOrder.Parameters.Add(idWebinarParameter);
                    sendOrder.Parameters.Add(idOptionParameter);
                    sendOrder.Parameters.Add(idDiscountParameter);
                    sendOrder.Parameters.Add(addLocParameter);
                    sendOrder.Parameters.Add(addLocCountParameter);


                    sendOrder.Connection = sqlConnection;
                    sendOrder.CommandType = CommandType.StoredProcedure;

                    orderid = sendOrder.ExecuteScalar().ToString();

                    using (var errorLogger = new SqlCommand("logError", sqlConnection))
                    {
                        errorLogger.CommandText =
                            "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                        errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() + "',";
                        errorLogger.CommandText += "'ACSIMPORTER' ,";
                        errorLogger.CommandText += "0 ,0 ,0 ,'[InsertACSUser]', 0 ,";
                        errorLogger.CommandText += "'importOrder returned " + orderid.ToString() + "')";

                        errorLogger.ExecuteNonQuery();
                    }
                }
                int number = 0;
                var result = Int32.TryParse(orderid, out number);
                if (result) newOrder.idOrderLegacy = number;

            }
            return orderid;
        }

        private int getLegacyOptionID(int idRegType)
        {
            switch (idRegType)
            {
                case 205:
                    return 1;

                case 206:
                    return 16;

                case 207:
                    return 3;

                case 208:
                    return 17;

                case 209:
                    return 18;

                case 200:
                    return 27;

                case 201:
                    return 32;

                case 203:
                    return 33;

                case 202:
                    return 35;

                case 204:
                    return 36;

                case 221:
                    return 79;

                case 222:
                    return 80;

                case 224:
                    return 81;

                case 223:
                    return 82;

                case 225:
                    return 83;

                default:
                    return 0;

            }
        }

        public Double[] GetCostOfUpgrades(int idRegType)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var getPricingsCommand = new SqlCommand())
                {

                }
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
    }
}
