using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core;

namespace CUWebinars.Web.Membership
{
    public class DataOperations
    {
        private readonly string _connectionString;

        public DataOperations()
        {
            GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
            _connectionString = globalConfig.MembershipConnectionString;
        }

        public bool SetNewAccountToVerified(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var updateUserCommand = new SqlCommand())
                {
                    var idParamater = new SqlParameter
                    {
                        Value = id,
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@ID"
                    };

                    updateUserCommand.Connection = sqlConnection;
                    updateUserCommand.Parameters.Add(idParamater);
                    updateUserCommand.CommandText = "UPDATE UserAccounts SET IsAccountVerified = 1 WHERE ID = @ID";
                    updateUserCommand.CommandType = CommandType.Text;

                    var numRows = updateUserCommand.ExecuteNonQuery();

                    return numRows.Equals(1);
                }
            }
        }

        public bool ManualPasswordReset(Guid id, string pwd)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var updateUserCommand = new SqlCommand())
                {
                    var idParamater = new SqlParameter
                    {
                        Value = id,
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@ID"
                    };

                    var pwdParamater = new SqlParameter
                    {
                        Value = pwd,
                        SqlDbType = SqlDbType.NVarChar,
                        Size = 200,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@PWD"
                    };


                    updateUserCommand.Connection = sqlConnection;
                    updateUserCommand.Parameters.AddRange(new SqlParameter[] { idParamater, pwdParamater });
                    updateUserCommand.CommandText = "UPDATE UserAccounts SET HashedPassword = @PWD WHERE ID = @ID";
                    updateUserCommand.CommandType = CommandType.Text;

                    var numRows = updateUserCommand.ExecuteNonQuery();

                    return numRows.Equals(1);
                }
            }
        }
        public string BuildACSImporter(Order order)
        {

            var myRow = order.OrderRows.FirstOrDefault();
            int AffiliateID = 62;
            int WebinarID = myRow.idWebinar;
            //int idRegType = _webinarManagementService.GetRegTypeByACS(order["DeliveryType"], Convert.ToInt32(order["BankWebID"]));
            int idRegType = 0;
            var FirstName = order.FirstName;
            var LastName = order.LastName;
            var Title = order.WebUser.Title;
            var Institution = order.WebUser.Institution.InstitutionName;
            var Email = order.BillingEmail;
            var Phone = order.BillingPhone;
            var Address = order.BillingAddress;
            var Address2 = "";
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
            var AffiliateComments = "ACSImporter";
            var OrderDate = order.OrderDate;
            var DeliveryType =myRow.idRegType.ToString();


            int intValue;


            var PostForm = ImportOrderModelQueryString(DeliveryType, AffiliateID, FirstName, LastName, Phone, Address, Address2, City, Zip, State, shippingFirstName, shippingLastName, shippingPhone, shippingAddress, shippingCity, shippingState, shippingZip, Email, Title, Institution, idRegType, WebinarID, AdditionalLocations, Convert.ToDateTime(OrderDate), DiscountCode);
            //var PostForm = MigrateOrderModelQueryString(AffiliateID, FirstName, LastName, Phone, Address, Address2, City, Zip, State, shippingFirstName, shippingLastName, shippingPhone, shippingAddress, shippingCity, shippingState, shippingZip, Email, Title, Institution, idRegType, WebinarID, AdditionalLocations, OrderDate, DiscountCode);

            string submitImporter = "http://localhost:51405/home/ImportOrder/";

            //Uri url = Request.Url;
            //var baseUri = new Uri(string.Concat(url.Scheme, @"://", url.Authority), UriKind.Absolute);
            //submitImporter = new Uri(
            //    baseUri,
            //    "Order/ImportOrder/"
            //    ).ToString();

            


            //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            //}


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
            return returnvalue;
        }

        private static string ImportOrderModelQueryString(string RegistrationType, int AffiliateID, string FirstName, string LastName, string Phone, string Address, string Address2, string City, string Zip, string State, string shippingFirstName, string shippingLastName, string shippingPhone, string shippingAddress, string shippingCity, string shippingState, string shippingZip, string Email, string Title, string Institution, int idRegType, int WebinarID, string AdditionalLocations, DateTime OrderDate, string DiscountCode)
        {
            var PostForm = "";

            PostForm = "idAffiliate=62&BillingAddress.AddressType=Billing";
            PostForm += "&BillingAddress.Name=" + System.Web.HttpUtility.UrlEncode(FirstName + " " + LastName);
            PostForm += "&Source=ACSGmailImport?Version=3";
            PostForm += "&BillingAddress.Phone=" + HttpUtility.UrlEncode(Phone);
            PostForm += "&BillingAddress.StreetAddress=" + HttpUtility.UrlEncode(Address);
            PostForm += "&BillingAddress.StreetAddress2=" + HttpUtility.UrlEncode(Address2);
            PostForm += "&BillingAddress.City=" + HttpUtility.UrlEncode(City);
            PostForm += "&BillingAddress.Zip=" + HttpUtility.UrlEncode(Zip);
            PostForm += "&BillingAddress.State=" + HttpUtility.UrlEncode(State);
            PostForm += "&BillingAddress.Country=" + HttpUtility.UrlEncode("US");
            PostForm += "&ShippingAddress.AddressType=Shipping";
            PostForm += "&ShippingAddress.Name=" + HttpUtility.UrlEncode(FirstName + " " + HttpUtility.UrlEncode(LastName));
            PostForm += "&ShippingAddress.Phone=" + HttpUtility.UrlEncode(shippingPhone);
            PostForm += "&ShippingAddress.StreetAddress=" + HttpUtility.UrlEncode(shippingAddress);
            PostForm += "&ShippingAddress.StreetAddress2=" + HttpUtility.UrlEncode("");
            PostForm += "&ShippingAddress.City=" + HttpUtility.UrlEncode(shippingCity);
            PostForm += "&ShippingAddress.State=" + HttpUtility.UrlEncode(shippingState);
            PostForm += "&ShippingAddress.Zip=" + HttpUtility.UrlEncode(shippingZip);
            PostForm += "&ShippingAddress.Country=" + HttpUtility.UrlEncode("US");
            PostForm += "&Email=" + HttpUtility.UrlEncode(Email);
            PostForm += "&Title=" + HttpUtility.UrlEncode(Title);
            PostForm += "&Institution=" + HttpUtility.UrlEncode(Institution);
            PostForm += "&FirstName=" + HttpUtility.UrlEncode(FirstName);
            PostForm += "&LastName=" + HttpUtility.UrlEncode(LastName);
            PostForm += "&RegistrationType=" + HttpUtility.UrlEncode(RegistrationType);
            PostForm += "&idWebinar=" + WebinarID;
            //PostForm += "&idOrderLegacy=" + HttpUtility.UrlEncode(1));
            PostForm += "&OrderDate=" + HttpUtility.UrlEncode(OrderDate.ToString());
            //'PostForm += "&idUserLegacy=" + HttpUtility.UrlEncode(29));
            PostForm += "&DiscountCode=" + HttpUtility.UrlEncode(DiscountCode);
            //'PostForm += "&Total=" + HttpUtility.UrlEncode(21));
            PostForm += "&AdditionalLocationsString=" + HttpUtility.UrlEncode(AdditionalLocations);

            return PostForm;
        }

    }
}