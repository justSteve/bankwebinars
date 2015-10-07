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
using System.Security.Claims;
using System.Text;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Extensions;

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
                    return Convert.ToDecimal(message);
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


        //public void SendOrderToLegacy(Order order)
        //{
        //    var myRow = order.OrderRows.FirstOrDefault();
        //    var addLoc = "";
        //    var addLocCount = 0;
        //    var orderid = "0";
        //    int translatedOptionId = 0;
        //    if (myRow.RegistrationType != null)
        //    {
        //        translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);
        //    }
        //    else
        //    {
        //        translatedOptionId = getLegacyOptionID(myRow.idRegType);
        //    }
        //    //int translatedOptionId = getLegacyOptionID(myRow.RegistrationType.idRegType);

        //    if (myRow.AdditionalLocation != null)
        //    {
        //        addLocCount = myRow.AdditionalLocation.Count;
        //        foreach (var loc in myRow.AdditionalLocation)
        //        {
        //            addLoc += loc.Email + ",";
        //        }
        //    }


        //    int AffiliateID = order.idAffiliate;
        //    int WebinarID = 0;
        //    //int idRegType = _webinarManagementService.GetRegTypeByACS(dic["DeliveryType"], Convert.ToInt32(dic["BankWebID"]));
        //    int idRegType = 0;
        //    var FirstName = order.FirstName;
        //    var LastName = order.LastName;
        //    var Title = order.WebUser.Title;
        //    var Institution = order.Institution;
        //    var Email = order.BillingEmail;
        //    var Phone = order.BillingPhone;
        //    var Address = order.BillingAddress;
        //    var Address2 = order.BillingAddress2;
        //    var City = order.BillingCity;
        //    var State = order.BillingState;
        //    var Zip = order.BillingZip;
        //    var DiscountCode = "";
        //    var AdditionalLocations = "";
        //    var shippingFirstName = order.FirstName;
        //    var shippingLastName = order.LastName;
        //    var shippingPhone = order.ShippingPhone;
        //    var shippingAddress = order.ShippingAddress;
        //    var shippingCity = order.ShippingCity;
        //    var shippingState = order.ShippingState;
        //    var shippingZip = order.ShippingZip;
        //    var AffiliateComments = "V3Migrator";
        //    var OrderDate = order.OrderDate;
        //    var DeliveryType = translatedOptionId;


        //    WebinarID = myRow.idWebinar;

        //    var PostForm = MigrateOrderModelQueryString(DeliveryType, AffiliateID, FirstName, LastName, Phone,
        //        Address, Address2, City, Zip, State, shippingFirstName, shippingLastName, shippingPhone,
        //        shippingAddress, shippingCity, shippingState, shippingZip, Email, Title, Institution, idRegType,
        //        WebinarID, AdditionalLocations, Convert.ToDateTime(OrderDate), DiscountCode);


        //    //string submitImporter = "http://localhost:51405/home/migrateorder";
        //    string submitImporter = "http://acsimporter.bankwebinars.com/home/migrateorder";


        //    WebRequest req = WebRequest.Create(submitImporter);

        //    byte[] send = Encoding.Default.GetBytes(PostForm);
        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-form-urlencoded";
        //    req.ContentLength = send.Length;

        //    Stream sout = req.GetRequestStream();
        //    sout.Write(send, 0, send.Length);
        //    sout.Flush();
        //    sout.Close();

        //    //WebResponse res = req.GetResponse();
        //    //StreamReader sr = new StreamReader(res.GetResponseStream());
        //    //string returnvalue = sr.ReadToEnd();

        //    // Display the content.
        //    //return returnvalue;

        //}

        //private string MigrateOrderModelQueryString(int deliveryType, int affiliateId, string firstName, string lastName, string phone, string address1, string address2, string city, string zip, string state, string shippingFirstName, string shippingLastName, string shippingPhone, string shippingAddress, string shippingCity, string shippingState, string shippingZip, string email, string title, string institution, int idRegType, int webinarId, string additionalLocations, DateTime orderDate, string discountCode)
        //{
        //    var PostForm = "";

        //    PostForm = "affiliateId=" + affiliateId + "&BillingAddress.AddressType=Billing";
        //    PostForm += "&BillingAddress.Name=" + HttpUtility.UrlEncode(firstName + " " + lastName);
        //    PostForm += "&Source=V3Migrator?Version=3";
        //    PostForm += "&phone=" + HttpUtility.UrlEncode(phone);
        //    PostForm += "&address1=" + HttpUtility.UrlEncode(address1);
        //    PostForm += "&address2=" + HttpUtility.UrlEncode(address2);
        //    PostForm += "&city=" + HttpUtility.UrlEncode(city);
        //    PostForm += "&zip=" + HttpUtility.UrlEncode(zip);
        //    PostForm += "&state=" + HttpUtility.UrlEncode(state);
        //    PostForm += "&ShippingFirstName=" + HttpUtility.UrlEncode(shippingFirstName);
        //    PostForm += "&ShippingLastName=" + HttpUtility.UrlEncode(HttpUtility.UrlEncode(shippingLastName));
        //    PostForm += "&ShippingPhone=" + HttpUtility.UrlEncode(shippingPhone);
        //    PostForm += "&ShippingAddress=" + HttpUtility.UrlEncode(shippingAddress);
        //    PostForm += "&ShippingCity=" + HttpUtility.UrlEncode(shippingCity);
        //    PostForm += "&ShippingState=" + HttpUtility.UrlEncode(shippingState);
        //    PostForm += "&ShippingZip=" + HttpUtility.UrlEncode(shippingZip);
        //    PostForm += "&email=" + HttpUtility.UrlEncode(email);
        //    PostForm += "&title=" + HttpUtility.UrlEncode(title);
        //    PostForm += "&institution=" + HttpUtility.UrlEncode(institution);
        //    PostForm += "&firstName=" + HttpUtility.UrlEncode(firstName);
        //    PostForm += "&lastName=" + HttpUtility.UrlEncode(lastName);
        //    PostForm += "&deliveryType=" + deliveryType;
        //    PostForm += "&webinarId=" + webinarId;
        //    PostForm += "&orderDate=" + HttpUtility.UrlEncode(orderDate.ToString());
        //    PostForm += "&discountCode=" + HttpUtility.UrlEncode(discountCode);
        //    PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(additionalLocations);

        //    return PostForm;

        //}


        public int getLegacyOptionID(int idRegType)
        {
            switch (idRegType)
            {
                case 4:
                    return 119;
                //trial CP
                case 5:
                    return 120;
                //6month
                case 38:
                    return 118;
                //12month
                case 200: return 27;
                //Live Plus Five (days) ;
                //PreEvent_1Hr_2013;
                case 201: return 32;
                //OnDemand Recording Only ;
                //PreEvent_1Hr_2013;
                case 203: return 33;
                //Live Plus Six (months) ;
                //PreEvent_1Hr_2013;
                case 202: return 35;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_1Hr_2013;
                case 204: return 36;
                //Premier Package ;
                //PreEvent_1Hr_2013;

                //2hr;
                ////;
                case 205: return 1;
                //Live Plus Five (days) ;
                //PreEvent_2Hr_2013
                case 206: return 16;
                //OnDemand Recording Only ;
                //PreEvent_2Hr_2013
                case 208: return 17;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_2Hr_2013
                case 209: return 18;
                //Premier Package ;
                //PreEvent_2Hr_2013
                case 207: return 3;
                //Live Plus Six (months) ;
                //PreEvent_2Hr_2013
                //2part;
                ////;
                case 249: return 85;
                //Live Plus Five (days) ;
                //PreEvent_2PartSeries_2014;
                case 250: return 86;
                //OnDemand Recording Only ;
                //PreEvent_2PartSeries_2014;
                case 253: return 87;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_2PartSeries_2014;
                case 251: return 88;
                //Live Plus Six ;
                //PreEvent_2PartSeries_2014;
                case 252: return 89;
                //Premier Package ;
                //PreEvent_2PartSeries_2014;

                ////;
                //3part;
                ////;
                case 210: return 48;
                //Live Plus Five (days) - 3 Part Series ;
                //PreEvent_Series3
                case 211: return 49;
                //On-Demand Recording Only ;
                //PreEvent_Series3
                case 213: return 50;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_Series3
                case 214: return 51;
                //Premium Package - Series ;
                //PreEvent_Series3
                case 212: return 91;
                //Live Plus Six (months) ;
                //PreEvent_Series3 id=26    ;

                ////;
                //4part;
                ////;
                case 216: return 39;
                //Live Only - 4 Part Series ;
                //PreEvent_4PartSeries_899
                case 217: return 40;
                //6-Month OnDemand Weblink - Series ;
                //PreEvent_4PartSeries_899
                case 219: return 41;
                //CD-ROM and Hardcopy Handouts - Series ;
                //PreEvent_4PartSeries_899
                case 220: return 42;
                //Premium Package - Series ;
                //PreEvent_4PartSeries_899
                case 218: return 71;
                //Live plus OnDemand Weblinks ;
                //PreEvent_4PartSeries_899

                ////;
                //5part;
                ////;
                case 221: return 79;
                //Live Plus Five (days) ;
                //PreEvent_5PartSeries_2014
                case 222: return 80;
                //OnDemand Recording Only ;
                //PreEvent_5PartSeries_2014
                case 224: return 81;
                //CD-ROM and Hardcopy Handouts ;
                //PreEvent_5PartSeries_2014
                case 223: return 82;
                //Live Plus Six ;
                //PreEvent_5PartSeries_2014
                case 225: return 83;
                //Premier Package ;
                //PreEvent_5PartSeries_2014
                default:
                    _logger.Fatal("Invalid Regtype detected at getLegacyOptionID!! {0}", idRegType);
                    return idRegType;



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
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts.ToString() +
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

        public List<Order> GetLegacyOrdersByWebinar(int? webinarId)
        {
            var idWebinarParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idWebinar", Value = webinarId };

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getOrder = new SqlCommand("GetLegacyOrders", sqlConnection))
                {
                    getOrder.Parameters.Add(idWebinarParameter);

                    List<Order> orders = new List<Order>();

                    try
                    {
                        getOrder.Connection = sqlConnection;
                        getOrder.CommandType = CommandType.StoredProcedure;

                        using (var reader = getOrder.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                try
                                {
                                    Order newOrder = new Order { OrderRows = new List<OrderRow>() };
                                    newOrder.OrderRows.Add(new OrderRow());
                                    var newOrderRow = newOrder.OrderRows.FirstOrDefault();
                                    newOrder.idAffiliate = reader.GetInt32(0);
                                    newOrderRow.idWebinar = reader.GetInt32(1);
                                    newOrderRow.idRegType = reader.GetInt32(2);
                                    newOrder.FirstName = reader.GetString(3);
                                    newOrder.LastName = reader.GetString(4);
                                    //skips title reader(5)
                                    newOrder.Institution = reader.GetString(6);
                                    newOrder.BillingEmail = reader.GetString(7);
                                    newOrder.BillingPhone = reader.GetString(8);
                                    newOrder.BillingAddress = reader.GetString(9);
                                    newOrder.BillingAddress2 = reader.GetString(10);
                                    newOrder.BillingCity = reader.GetString(11);
                                    newOrder.BillingState = reader.GetString(12);
                                    newOrder.BillingZip = reader.GetString(13);
                                    if (!String.IsNullOrEmpty(reader[14].ToString()))
                                    {
                                        newOrderRow.Discount = new Discount { DiscountCode = reader.GetString(14) };
                                    }
                                    if (!String.IsNullOrEmpty(reader[15].ToString()))
                                    {
                                        var addLoc = reader.GetString(15).Split(',');
                                        newOrderRow.AdditionalLocation = new List<AdditionalLocation>();
                                        if (addLoc.Count() == 1)
                                        {
                                            AdditionalLocation additional = new AdditionalLocation { Email = reader.GetString(15) };

                                            newOrderRow.AdditionalLocation.Add(additional);
                                            //addLocations.Add(additional);
                                        }
                                        else
                                        {
                                            foreach (var loc in addLoc)
                                            {
                                                AdditionalLocation additional = new AdditionalLocation { Email = loc };
                                                newOrderRow.AdditionalLocation.Add(additional);

                                            }
                                        }
                                    }


                                    newOrder.ShippingFirstName = reader.GetString(16);
                                    newOrder.ShippingLastName = reader.GetString(17);
                                    newOrder.ShippingPhone = reader.GetString(18);
                                    newOrder.ShippingAddress = reader.GetString(19);
                                    newOrder.ShippingCity = reader.GetString(20);
                                    newOrder.ShippingState = reader.GetString(21);
                                    newOrder.ShippingZip = reader.GetString(22);
                                    newOrder.Total = reader.GetDecimal(23);
                                    //skips shipdate 24
                                    newOrder.AdminComments = reader.GetString(25);
                                    newOrder.idOrderLegacy = reader.GetInt32(26);
                                    newOrder.OrderDate = reader.GetDateTime(27);

                                    newOrder.Origin = "OrderSynch";
                                    int mkStatus = Convert.ToInt32(reader[28]);

                                    var setStatus = SetOrderStatus(mkStatus);
                                    newOrder.UserComments = reader.GetInt32(29).ToString();
                                    newOrder.OrderStatus = setStatus;

                                    newOrderRow.RowStatus = OrderRowStatus.Active;
                                    orders.Add(newOrder);
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error("GetLegacyOrders hit error on: " + reader.GetString(7) + " msg: " + ex.Message);
                                }
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
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts +
                                                       "',";
                            errorLogger.CommandText += "'GetLegacyOrders' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[GetLegacyOrders]', 9 ,";
                            errorLogger.CommandText += "'error at GetLegacyOrders " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                        return null;
                    }
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

        public void MigrateOrderFromV3(Order order)
        {
            if (order == null) return;

            var myRow = order.OrderRows.SingleOrDefault();
            string myDiscount = "";
            if (myRow.Discount != null)
            {
                myDiscount = myRow.Discount.DiscountCode;
            }

            var PostForm = "";

            PostForm = "affiliateId=" + order.idAffiliate;
            PostForm += "&firstName=" + order.FirstName;
            PostForm += "&lastName=" + order.LastName;
            PostForm += "&phone=" + order.BillingPhone;
            PostForm += "&address1=" + order.BillingAddress;
            PostForm += "&address2=" + order.BillingAddress2;
            PostForm += "&city=" + order.BillingCity;
            PostForm += "&zip=" + order.BillingZip;
            PostForm += "&state=" + order.BillingState;
            PostForm += "&shippingFirstName=" + order.ShippingFirstName;
            PostForm += "&shippingLastname=" + order.ShippingLastName;
            PostForm += "&shippingPhone=" + order.ShippingPhone;
            PostForm += "&shippingAddress=" + order.ShippingAddress;
            PostForm += "&shippingCity=" + order.ShippingCity;
            PostForm += "&shippingState=" + order.ShippingState;
            PostForm += "&shippingZip=" + order.ShippingZip;
            PostForm += "&Email=" + order.BillingEmail;
            PostForm += "&Title=" + "";
            PostForm += "&Institution=" + order.Institution;
            PostForm += "&idRegType=" + myRow.idRegType;
            PostForm += "&webinarId=" + myRow.idWebinar;
            PostForm += "&AdditionalLocationsString=" + myRow.AdditionalLocation;
            PostForm += "&OrderDate=" + order.OrderDate;
            PostForm += "&DiscountCode=" + myDiscount;
            PostForm += "&Status=" + order.OrderStatus + "&Total=" + order.Total;
            PostForm += "&AdminComments=" + order.AdminComments;

            var submitImporter = "http://acsimporter.bankwebinars.com/home/MigrateOrderFromV3/";
            if (Debugger.IsAttached)
            {

                submitImporter = "http://localhost:51405/home/MigrateOrderFromV3/";
            }

            WebRequest req = WebRequest.Create(submitImporter);

            byte[] send = Encoding.Default.GetBytes(PostForm);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            // Display the content.
            //return returnvalue;
            ;
            SynchOrderIds(Convert.ToInt32(returnvalue.Split(':')[1].Replace("\"", "").Replace("}", "")), order.idOrder);

        }

        private void SynchOrderIds(int legacyOrderId, int v3OrderId)
        {
            using (var sqlConnection = new SqlConnection(TtsConfig.DefaultConnectionString))
            {
                sqlConnection.Open();
                var v3idOrderParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idOrderV3", Value = v3OrderId };
                var idOrderParameter = new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@idOrder", Value = legacyOrderId };

                using (var synchOrderIds = new SqlCommand("SynchOrderIds", sqlConnection))
                {
                    synchOrderIds.Parameters.Add(v3idOrderParameter);
                    synchOrderIds.Parameters.Add(idOrderParameter);

                    try
                    {
                        synchOrderIds.Connection = sqlConnection;
                        synchOrderIds.CommandType = CommandType.StoredProcedure;

                        synchOrderIds.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        using (var errorLogger = new SqlCommand("logError", sqlConnection))
                        {
                            errorLogger.CommandText =
                                "INSERT dbo.ErrorLog ( ErrorTime ,UserName ,ErrorNumber ,ErrorSeverity ,ErrorState ,ErrorProcedure ,ErrorLine ,ErrorMessage)VALUES  ('";
                            errorLogger.CommandText += DomainConstants.BuildUtcNowAsCts +
                                                       "',";
                            errorLogger.CommandText += "'SynchOrderIds' ,";
                            errorLogger.CommandText += "9 ,9 ,9 ,'[SynchOrderIds]', 9 ,";
                            errorLogger.CommandText += "'error at SynchOrderIds " + ex.Message + "')";

                            errorLogger.ExecuteNonQuery();

                        }
                    }
                }

            }
        }

        public void MigrateOrderFromLegacy(Order order)
        {
            if (order == null) return;

            var myRow = order.OrderRows.SingleOrDefault();
            string myDiscount = "";
            if (myRow.Discount != null)
            {
                myDiscount = myRow.Discount.DiscountCode;
            }
            var addLocs = "";


            if (myRow.AdditionalLocation != null && myRow.AdditionalLocation.Count > 0)
            {
                foreach (var additionalLocation in myRow.AdditionalLocation)
                {
                    addLocs += additionalLocation.Email + ",";
                }
            }

            var PostForm = "";

            PostForm = "idAffiliate=" + order.idAffiliate + "&BillingAddress.AddressType=Billing";


            PostForm += "&BillingAddress.Name=" + order.FirstName + " " + order.LastName;
            PostForm += "&BillingAddress.Phone=" + order.BillingPhone;
            PostForm += "&BillingAddress.StreetAddress=" + order.BillingAddress;
            PostForm += "&BillingAddress.StreetAddress2=" + order.BillingAddress2;
            PostForm += "&BillingAddress.City=" + order.BillingCity;
            PostForm += "&BillingAddress.Zip=" + order.BillingZip;
            PostForm += "&BillingAddress.State=" + order.BillingState;
            PostForm += "&BillingAddress.Country=" + "US";
            PostForm += "&ShippingAddress.AddressType=Shipping";
            PostForm += "&ShippingAddress.Name=" + order.ShippingFirstName + " " + order.ShippingLastName;
            PostForm += "&ShippingAddress.Phone=" + order.ShippingPhone;
            PostForm += "&ShippingAddress.StreetAddress=" + order.ShippingAddress;
            PostForm += "&ShippingAddress.StreetAddress2=" + "";
            PostForm += "&ShippingAddress.City=" + order.ShippingCity;
            PostForm += "&ShippingAddress.State=" + order.ShippingState;
            PostForm += "&ShippingAddress.Zip=" + order.ShippingZip;
            PostForm += "&ShippingAddress.Country=" + "US";
            PostForm += "&Email=" + order.BillingEmail;
            PostForm += "&Title=" + "";
            PostForm += "&Institution=" + order.Institution;
            PostForm += "&FirstName=" + order.FirstName;
            PostForm += "&LastName=" + order.LastName;
            PostForm += "&idRegType=" + myRow.idRegType;
            PostForm += "&idWebinar=" + myRow.idWebinar;
            PostForm += "&AdditionalLocationsString=" + addLocs.TrimEnd(' ', ',');
            PostForm += "&idOrderLegacy=" + order.idOrderLegacy;
            PostForm += "&OrderDate=" + order.OrderDate;
            PostForm += "&ShippingDate=" + "";
            PostForm += "&DiscountCode=" + myDiscount;
            PostForm += "&Status=" + order.OrderStatus + "&Total=" + order.Total;
            PostForm += "&AdminComments=" + order.AdminComments;

            PostForm = PostForm.Replace("<br>", "");
            var submitImporter = "http://v3.bankwebinars.com/order/MigrateOrder/";
            if (Debugger.IsAttached)
            {
                submitImporter = "http://localhost:3538/order/MigrateOrder/";
            }
            WebRequest req = WebRequest.Create(submitImporter);

            byte[] send = Encoding.Default.GetBytes(PostForm);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            // Display the content.
            //return returnvalue;

            SynchOrderIds(order.idOrderLegacy, Convert.ToInt32(returnvalue.Split(':')[1].Replace("\"", "").Replace("}", "")));
            ;

        }

    }
}
