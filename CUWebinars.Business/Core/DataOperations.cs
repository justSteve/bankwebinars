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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using CUWebinars.Business.Constants;

namespace CUWebinars.Business.Core
{
    public class DataOperations
    {
        private readonly string _connectionString;

        private readonly ILogger _logger;

        private readonly TtsConfiguration _ttsConfig;

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
                        _logger.Warn("Unknown Registration Type: " + getLegacyWebinars.CommandText +
                                     " Exception.Message: " + ex.Message);
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
                            pricingInformation.Add(new AdditionalLocationsPricing
                            {
                                LookupPriceId = reader.GetInt32(0),
                                Price = reader.GetDecimal(1)
                            });
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

        public void SendOrderToLegacy(Order order)
        {
            var myRow = order.OrderRows.FirstOrDefault();
            var addLoc = "";
            var addLocCount = 0;
            var orderid = "0";
            int translatedOptionId = 0;
            if (myRow.RegistrationType != null)
            {
                translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);
            }
            else
            {
                translatedOptionId = getLegacyOptionID(myRow.idRegType);
            }
            //int translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);

            if (myRow.AdditionalLocation != null)
            {
                addLocCount = myRow.AdditionalLocation.Count;
                foreach (var loc in myRow.AdditionalLocation)
                {
                    addLoc += loc.Email + ",";
                }
            }


            int AffiliateID = order.idAffiliate;
            int WebinarID = 0;
            //int idRegType = _webinarManagementService.GetRegTypeByACS(dic["DeliveryType"], Convert.ToInt32(dic["BankWebID"]));
            int idRegType = 0;
            var FirstName = order.FirstName;
            var LastName = order.LastName;
            var Title = order.WebUser.Title;
            var Institution = order.Institution;
            var Email = order.BillingEmail;
            var Phone = order.BillingPhone;
            var Address = order.BillingAddress;
            var Address2 = order.BillingAddress2;
            var City = order.BillingCity;
            var State = order.BillingState;
            var Zip = order.BillingZip;
            var DiscountCode = "";
            var AdditionalLocations = "";
            var shippingFirstName = order.FirstName;
            var shippingLastName = order.LastName;
            var shippingPhone = order.ShippingPhone;
            var shippingAddress = order.ShippingAddress;
            var shippingCity = order.ShippingCity;
            var shippingState = order.ShippingState;
            var shippingZip = order.ShippingZip;
            var AffiliateComments = "V3Migrator";
            var OrderDate = order.OrderDate;
            var DeliveryType = translatedOptionId;


            WebinarID = myRow.idWebinar;

            var PostForm = MigrateOrderModelQueryString(DeliveryType, AffiliateID, FirstName, LastName, Phone,
                Address, Address2, City, Zip, State, shippingFirstName, shippingLastName, shippingPhone,
                shippingAddress, shippingCity, shippingState, shippingZip, Email, Title, Institution, idRegType,
                WebinarID, AdditionalLocations, Convert.ToDateTime(OrderDate), DiscountCode);


            //string submitImporter = "http://localhost:51405/home/migrateorder";
            string submitImporter = "http://acsimporter.bankwebinars.com/home/migrateorder";


            WebRequest req = WebRequest.Create(submitImporter);

            byte[] send = Encoding.Default.GetBytes(PostForm);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            //WebResponse res = req.GetResponse();
            //StreamReader sr = new StreamReader(res.GetResponseStream());
            //string returnvalue = sr.ReadToEnd();

            // Display the content.
            //return returnvalue;

        }

        private string MigrateOrderModelQueryString(int deliveryType, int affiliateId, string firstName, string lastName, string phone, string address1, string address2, string city, string zip, string state, string shippingFirstName, string shippingLastName, string shippingPhone, string shippingAddress, string shippingCity, string shippingState, string shippingZip, string email, string title, string institution, int idRegType, int webinarId, string additionalLocations, DateTime orderDate, string discountCode)
        {
            var PostForm = "";

            PostForm = "affiliateId=" + affiliateId + "&BillingAddress.AddressType=Billing";
            PostForm += "&BillingAddress.Name=" + HttpUtility.UrlEncode(firstName + " " + lastName);
            PostForm += "&Source=V3Migrator?Version=3";
            PostForm += "&phone=" + HttpUtility.UrlEncode(phone);
            PostForm += "&address1=" + HttpUtility.UrlEncode(address1);
            PostForm += "&address2=" + HttpUtility.UrlEncode(address2);
            PostForm += "&city=" + HttpUtility.UrlEncode(city);
            PostForm += "&zip=" + HttpUtility.UrlEncode(zip);
            PostForm += "&state=" + HttpUtility.UrlEncode(state);
            PostForm += "&ShippingFirstName=" + HttpUtility.UrlEncode(shippingFirstName);
            PostForm += "&ShippingLastName=" + HttpUtility.UrlEncode(HttpUtility.UrlEncode(shippingLastName));
            PostForm += "&ShippingPhone=" + HttpUtility.UrlEncode(shippingPhone);
            PostForm += "&ShippingAddress=" + HttpUtility.UrlEncode(shippingAddress);
            PostForm += "&ShippingCity=" + HttpUtility.UrlEncode(shippingCity);
            PostForm += "&ShippingState=" + HttpUtility.UrlEncode(shippingState);
            PostForm += "&ShippingZip=" + HttpUtility.UrlEncode(shippingZip);
            PostForm += "&email=" + HttpUtility.UrlEncode(email);
            PostForm += "&title=" + HttpUtility.UrlEncode(title);
            PostForm += "&institution=" + HttpUtility.UrlEncode(institution);
            PostForm += "&firstName=" + HttpUtility.UrlEncode(firstName);
            PostForm += "&lastName=" + HttpUtility.UrlEncode(lastName);
            PostForm += "&deliveryType=" + deliveryType;
            PostForm += "&webinarId=" + webinarId;
            PostForm += "&orderDate=" + HttpUtility.UrlEncode(orderDate.ToString());
            PostForm += "&discountCode=" + HttpUtility.UrlEncode(discountCode);
            PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(additionalLocations);

            return PostForm;

        }

        public
            string SendOrderToLegacyForSQL(Order newOrder)
        {
            //attempting to deprecate this
            var myRow = newOrder.OrderRows.FirstOrDefault();
            var addLoc = "";
            var addLocCount = 0;
            var orderid = "0";
            int translatedOptionId = 0;
            if (myRow.RegistrationType != null)
            {
                translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);
            }
            else
            {
                translatedOptionId = getLegacyOptionID(myRow.idRegType);
            }
            //int translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);

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

            var idAffParameter = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@idAffiliate",
                Value = newOrder.idAffiliate
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

                if (newOrder.idAffiliate == 62)
                {
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
                                errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() +
                                                           "',";
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
                }
                else
                {
                    using (var getUserID = new SqlCommand("InsertUser", sqlConnection))
                    {
                        getUserID.Parameters.Add(firstNameParameter);
                        getUserID.Parameters.Add(idAffParameter);
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
                                errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() +
                                                           "',";
                                errorLogger.CommandText += "'CartSynch' ,";
                                errorLogger.CommandText += "9 ,9 ,9 ,'[InsertUser]', 9 ,";
                                errorLogger.CommandText += "'error at InsertUser " + ex.Message + "')";

                                errorLogger.ExecuteNonQuery();

                            }
                        }
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() + "',";
                            errorLogger.CommandText += "'CartSynch' ,";
                            errorLogger.CommandText += "0 ,0 ,0 ,'[InsertUser]', 0 ,";
                            errorLogger.CommandText += "'InsertUser returned " + idUser.ToString() + "')";

                            errorLogger.ExecuteNonQuery();
                        }
                    }
                }
                if (newOrder.idAffiliate == 62)
                {
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
                }
                else
                {
                    using (var sendOrder = new SqlCommand("MigrateOrder", sqlConnection))
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
                            errorLogger.CommandText += "'CartSynch' ,";
                            errorLogger.CommandText += "0 ,0 ,0 ,'[MigrateOrder]', 0 ,";
                            errorLogger.CommandText += "'MigrateOrder returned " + orderid.ToString() + "')";

                            errorLogger.ExecuteNonQuery();
                        }
                    }

                }
                int number = 0;
                var result = Int32.TryParse(orderid, out number);
                if (result) newOrder.idOrderLegacy = number;

            }
            return orderid;
        }

        public int getLegacyOptionID(int idRegType)
        {
            switch (idRegType)
            {
                case 200: return 27;
                //Live Plus Five (days) ;
                //PreEvent_1Hr_2013 id=29;
                case 201: return 32;
                //OnDemand Recording Only ;
                //PreEvent_1Hr_2013 id=29;
                case 203: return 33;
                //Live Plus Six (months) ;
                //PreEvent_1Hr_2013 id=29;
                case 202: return 35;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_1Hr_2013 id=29;
                case 204: return 36;
                //Premier Package ;
                //PreEvent_1Hr_2013 id=29;

                //2hr;
                ////;
                case 205: return 1;
                //Live Plus Five (days) ;
                //PreEvent_2Hr_2013 id=27;
                case 206: return 16;
                //OnDemand Recording Only ;
                //PreEvent_2Hr_2013 id=27;
                case 208: return 17;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_2Hr_2013 id=27;
                case 209: return 18;
                //Premier Package ;
                //PreEvent_2Hr_2013 id=27;
                case 207: return 3;
                //Live Plus Six (months) ;
                //PreEvent_2Hr_2013 id=27;
                //2part;
                ////;
                case 249: return 85;
                //Live Plus Five (days) ;
                //PreEvent_2PartSeries_2014 id=34;
                case 250: return 86;
                //OnDemand Recording Only ;
                //PreEvent_2PartSeries_2014 id=34;
                case 253: return 87;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_2PartSeries_2014 id=34;
                case 251: return 88;
                //Live Plus Six ;
                //PreEvent_2PartSeries_2014 id=34;
                case 252: return 89;
                //Premier Package ;
                //PreEvent_2PartSeries_2014 id=34;

                ////;
                //3part;
                ////;
                case 210: return 48;
                //Live Plus Five (days) - 3 Part Series ;
                //PreEvent_Series3 id=26;
                case 211: return 49;
                //On-Demand Recording Only ;
                //PreEvent_Series3 id=26;
                case 213: return 50;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_Series3 id=26;
                case 214: return 51;
                //Premium Package - Series ;
                //PreEvent_Series3 id=26;
                case 212: return 91;
                //Live Plus Six (months) ;
                //PreEvent_Series3 id=26    ;

                ////;
                //4part;
                ////;
                case 216: return 39;
                //Live Only - 4 Part Series ;
                //PreEvent_4PartSeries_899 id=23;
                case 217: return 40;
                //6-Month OnDemand Weblink - Series ;
                //PreEvent_4PartSeries_899 id=23;
                case 219: return 41;
                //CD-ROM and Hardcopy Handouts - Series ;
                //PreEvent_4PartSeries_899 id=23;
                case 220: return 42;
                //Premium Package - Series ;
                //PreEvent_4PartSeries_899 id=23;
                case 218: return 71;
                //Live plus OnDemand Weblinks ;
                //PreEvent_4PartSeries_899 id=23;

                ////;
                //5part;
                ////;
                case 221: return 79;
                //Live Plus Five (days) ;
                //PreEvent_5PartSeries_2014 id=32;
                case 222: return 80;
                //OnDemand Recording Only ;
                //PreEvent_5PartSeries_2014 id=32;
                case 224: return 81;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_5PartSeries_2014 id=32;
                case 223: return 82;
                //Live Plus Six ;
                //PreEvent_5PartSeries_2014 id=32;
                case 225: return 83;
                //Premier Package ;
                //PreEvent_5PartSeries_2014 id=32;
                default:
                    _logger.Fatal("Invalid Regtype detected at getLegacyOptionID!! {0}", idRegType);
                    return 99000 + idRegType;



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
                        "DELETE FROM dbo.UserClaims WHERE Type = 'http://ttstrain.com/ws/2014/01/identity/claims/BeingImpersonated' AND Value = @adminEmail;";

                    numRows = sqlCmd.ExecuteNonQuery();
                }
            }

        }

        public OrderRow GetLegacyOrder(Order order)
        {
            var idWebinarParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idWebinar", Value = order.OrderRows.FirstOrDefault().idWebinar };
            var emailParameter = new SqlParameter { SqlDbType = SqlDbType.VarChar, Size = 200, ParameterName = "@email", Value = order.BillingEmail };

            OrderRow lOrder = new OrderRow();
            lOrder.idOrder = 0;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {

                sqlConnection.Open();

                using (var getOrder = new SqlCommand("GetLegacyOrderForV3", sqlConnection))
                {
                    getOrder.Parameters.Add(idWebinarParameter);
                    getOrder.Parameters.Add(emailParameter);

                    try
                    {
                        getOrder.Connection = sqlConnection;
                        getOrder.CommandType = CommandType.StoredProcedure;

                        using (var reader = getOrder.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lOrder.idOrder = Convert.ToInt32(reader.GetInt32(0));
                                lOrder.idRegType = Convert.ToInt32(reader.GetInt32(1));
                                //lOrder.RowStatus = (OrderRowStatus)Convert.ToInt32(reader.GetInt32(2));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() +
                                                       "',";
                            errorLogger.CommandText += "'GETLEGACYORDER' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[GETLEGACYORDER]', 9 ,";
                            errorLogger.CommandText += "'error at GETLEGACYORDER " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
                return lOrder;
            }
        }

        public ListDictionary ImportLegacyOrders(int? webinarId)
        {
            var idWebinarParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idWebinar", Value = webinarId };
            //var emailParameter = new SqlParameter { SqlDbType = SqlDbType.VarChar, Size = 200, ParameterName = "@email", Value = order.BillingEmail };
            //var defaultConnection =
            //    "Data Source=tcp:nt2j4x3hvq.database.windows.net,1433;Initial Catalog=BankWebinars33_db;User Id=TTSOp@kmow9uloz6;Password=HXm88WIX;MultipleActiveResultSets=True;";

            using (var sqlConnection = new SqlConnection(_connectionString))
            {

                sqlConnection.Open();

                using (var getOrder = new SqlCommand("ImportLegacyOrders", sqlConnection))
                {
                    getOrder.Parameters.Add(idWebinarParameter);

                    ListDictionary orders = new ListDictionary();

                    try
                    {
                        getOrder.Connection = sqlConnection;
                        getOrder.CommandType = CommandType.StoredProcedure;

                        using (var reader = getOrder.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var idOrder = Convert.ToInt32(reader.GetInt32(0));
                                var email = reader.GetString(1);
                                orders.Add(email, idOrder);
                            }
                        }

                        return orders;
                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() +
                                                       "',";
                            errorLogger.CommandText += "'ImportLegacyOrders' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[ImportLegacyOrders]', 9 ,";
                            errorLogger.CommandText += "'error at ImportLegacyOrders " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                        return null;
                    }
                }
            }
        }

        public void ImportLegacyOrder(int value)
        {
            var idOrderParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idOrder", Value = value };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getOrder = new SqlCommand("MigrateLegacyOrderToV3", sqlConnection))
                {
                    getOrder.Parameters.Add(idOrderParameter);

                    try
                    {
                        using (var reader = getOrder.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var idOrder = Convert.ToInt32(reader.GetInt32(0));
                                var email = reader.GetString(1);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToShortDateString() +
                                                       "',";
                            errorLogger.CommandText += "'MigrateLegacyOrderToV3' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[MigrateLegacyOrderToV3]', 9 ,";
                            errorLogger.CommandText += "'error at MigrateLegacyOrderToV3 " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }
            }

        }
    }
}
