using System.Configuration;
using System.Net.Http.Headers;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System.Web.Http.ModelBinding;

namespace CUWebinars.Web.Controllers.api
{
    public class OrderImporterController : ApiController
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;
        private readonly ILogger _logger;

        public OrderImporterController(IMembershipService membershipService, IOrderManagementService orderManagementService, IStateService stateService, ILogger logger)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _logger = logger;
        }

        //[HttpPost()]
        public ImportOrderResponse ImportOrder(IncomingAFOOrder incomingAFOOrder)
        {
            string orderid = string.Empty, responseString;
            int number;
            bool isError = false;

            var incomingOrderModel = new IncomingOrderModel();

            try
            {

                incomingOrderModel.BillingAddress = new Address();
                incomingOrderModel.ShippingAddress = new Address();

                incomingOrderModel.AffiliateComments = incomingAFOOrder.SubmissionDate.ToString() + " " + incomingAFOOrder.IP;

                incomingOrderModel.BillingAddress.Name = string.Concat(incomingAFOOrder.FirstName, " ", incomingAFOOrder.LastName);
                incomingOrderModel.BillingAddress.Phone = incomingAFOOrder.Phone;
                incomingOrderModel.BillingAddress.StreetAddress = incomingAFOOrder.StreetAddress;
                incomingOrderModel.BillingAddress.StreetAddress2 = incomingAFOOrder.StreetAddress2;
                incomingOrderModel.BillingAddress.City = incomingAFOOrder.City;
                incomingOrderModel.BillingAddress.State = incomingAFOOrder.State;
                incomingOrderModel.BillingAddress.Zip = incomingAFOOrder.Zip;
                incomingOrderModel.BillingAddress.Country = incomingAFOOrder.Country;
                incomingOrderModel.BillingAddress.AddressType = "Billing";

                incomingOrderModel.ShippingAddress.Name = string.Concat(incomingAFOOrder.FirstName, " ", incomingAFOOrder.LastName);
                incomingOrderModel.ShippingAddress.Phone = incomingAFOOrder.Phone;
                incomingOrderModel.ShippingAddress.StreetAddress = incomingAFOOrder.StreetAddress;
                incomingOrderModel.ShippingAddress.StreetAddress2 = incomingAFOOrder.StreetAddress2;
                incomingOrderModel.ShippingAddress.City = incomingAFOOrder.City;
                incomingOrderModel.ShippingAddress.State = incomingAFOOrder.State;
                incomingOrderModel.ShippingAddress.Zip = incomingAFOOrder.Zip;
                incomingOrderModel.ShippingAddress.Country = incomingAFOOrder.Country;



                // Call API Url

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebinarUrl"]);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsJsonAsync(ConfigurationManager.AppSettings["actionOrder"], incomingAFOOrder).Result;

                if (response.IsSuccessStatusCode)
                {
                    responseString = response.Content.ReadAsStringAsync().Result;
                    orderid = JObject.Parse(responseString)["Result"].ToString();
                    if (!int.TryParse(orderid, out number))
                    {
                        isError = true;
                    }
                    else
                    {
                        if (orderid == "0")
                            isError = true;
                    }
                }
                else
                {
                    isError = true;
                    orderid = "Please try later.";
                }

                return new ImportOrderResponse()
                {
                    IsError = isError,
                    Status = orderid,//Convert.ToString("test"),
                    rowIndex = incomingAFOOrder.rowNumber
                };
            }
            catch (Exception ex)
            {
                Exception activeException = GeneralFunction.GetInnerMostException(ex);
                return new ImportOrderResponse()
                {
                    IsError = true,
                    Status = activeException.Message,//"Sorry, your feedback could not be sent",
                    rowIndex = incomingOrderModel.rowNumber
                };
            }
        }


        // POST api/<controller>
        public HttpResponseMessage Post([FromBody] IncomingOrderModel incomingOrderModel)
        {



            int idOfLastOrder = default(int);
            HttpResponseMessage httpResponseMessage = null;

            if (!ModelState.IsValid)
            {
                _logger.Error("Post found problem with Payload");
                var myErr = "";
                //foreach (ModelState modelState in ViewData.ModelState.Values)
                //{
                //    foreach (ModelError error in modelState.Errors)
                //    {
                //        myErr += modelState.Value.ToString();
                //    }
                //}

                myErr = string.Join(",", ModelState.Keys.SelectMany(k => ModelState[k].Errors).Select(m => m.ErrorMessage).ToArray());

                return Request.CreateResponse(HttpStatusCode.Found, new { Result = myErr });
            }

            try
            {
                //foreach (var incomingOrderModel in model)
                //{
                _logger.Info("Begin import: " + incomingOrderModel.Email.Trim());
                var email = incomingOrderModel.Email.Trim();

                var firstName = incomingOrderModel.FirstName.Trim();
                var lastName = incomingOrderModel.LastName.Trim();

                var webinar = _orderManagementService.GetWebinar(incomingOrderModel.idWebinar);
                var webUser = _membershipService.GetUserByEmail(email);
                var affiliate = _orderManagementService.GetAffiliateById(incomingOrderModel.idAffiliate);

                if (webUser == null)
                {
                    var institutionForUser =
                        _membershipService.ProcessInstitutionForUser(incomingOrderModel.Institution.Trim(),
                            incomingOrderModel.Email,
                            incomingOrderModel.BillingAddress.City,
                            incomingOrderModel.BillingAddress.State,
                            "N",
                            "New",
                            incomingOrderModel.BillingAddress.Zip
                            );

                    incomingOrderModel.BillingAddress.AddressType = WebUiConstants.BillingAddress;
                    incomingOrderModel.ShippingAddress.AddressType = WebUiConstants.ShippingAddress;

                    IList<Address> addresses = new List<Address>
                        {
                            incomingOrderModel.BillingAddress,
                            incomingOrderModel.ShippingAddress
                        };

                    USTimeZone userTimeZone = _membershipService.GetTimeZoneByZip();
                    webUser = _membershipService.CreateWebUser(globalConfig.Tenant
                        , firstName.Trim()
                        , lastName.Trim()
                        , lastName.ToLower().Trim()
                        , email.Trim()
                        , userTimeZone
                        , UserType.Customer
                        , institutionForUser.idInstitution
                        , addresses
                        , incomingOrderModel.Title == null ? incomingOrderModel.Title : incomingOrderModel.Title.Trim()
                        , null
                        , DomainConstants.Active
                        );

                    webUser.Institution = institutionForUser;

                    _membershipService.CreateUser(globalConfig.Tenant
                        , firstName
                        , lastName
                        , email
                        , lastName.ToLower().Trim()
                        , email
                        );
                }

                IList<AdditionalLocation> addLocation = new List<AdditionalLocation>();

                if (incomingOrderModel.AdditionalLocation != null && incomingOrderModel.AdditionalLocation.Any())
                {
                    var price = 150;
                    //string email,decimal price,string fullname
                    var additionalLocations = incomingOrderModel.AdditionalLocation;

                    foreach (var additionalLocation in additionalLocations)
                    {
                        var additionalLocationEmail = additionalLocation.Email;
                        //var additionalLocationFirstName = email.Split('@')[0].ToString(); // additionalLocation.FirstName
                        //var additionalLocationLastName = email.Split('@')[1].ToString(); // additionalLocation.LastName
                        addLocation.Add(_orderManagementService.CreateAdditionalLocation(
                            additionalLocationEmail,
                            price,
                            null) //field for FullName
                            //additionalLocationFirstName + ' ' + additionalLocationLastName)
                            );
                    }
                }

                var orderRow = _orderManagementService.CreateOrderRow(
                    webinar,
                    addLocation,
                    incomingOrderModel.idRegType
                    );

                orderRow.Discount = _orderManagementService.GetDiscount(incomingOrderModel.Email);

                var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

                importedOrder.AdminComments = "incomingOrderModel.AdminComments";
                importedOrder.AffiliateComments = incomingOrderModel.AffiliateComments;
                importedOrder.UserComments = "incomingOrderModel.UserComments";
                importedOrder.Origin = "incomingOrderModel.Origin";
                importedOrder.FirstName = webUser.FirstName;
                importedOrder.LastName = webUser.LastName;
                importedOrder.Institution = webUser.Institution.InstitutionName;
                importedOrder.BillingEmail = incomingOrderModel.Email;

                importedOrder.BillingAddress = incomingOrderModel.BillingAddress.StreetAddress;
                importedOrder.BillingAddress2 = incomingOrderModel.BillingAddress.StreetAddress2;
                importedOrder.BillingPhone = incomingOrderModel.BillingAddress.Phone;
                importedOrder.BillingCity = incomingOrderModel.BillingAddress.City;
                importedOrder.BillingState = incomingOrderModel.BillingAddress.State;
                importedOrder.BillingZip = incomingOrderModel.BillingAddress.Zip;

                importedOrder.ShippingAddress = incomingOrderModel.ShippingAddress.StreetAddress;
                importedOrder.ShippingAddress2 = incomingOrderModel.ShippingAddress.StreetAddress2;
                importedOrder.ShippingPhone = incomingOrderModel.ShippingAddress.Phone;
                importedOrder.ShippingCity = incomingOrderModel.ShippingAddress.City;
                importedOrder.ShippingState = incomingOrderModel.ShippingAddress.State;
                importedOrder.ShippingZip = incomingOrderModel.ShippingAddress.Zip;
                importedOrder.ShippingFirstName = firstName;
                importedOrder.ShippingLastName = lastName;

                if (orderRow.Webinar.WebinarKey != null)
                {

                    var regKeyResponse = _orderManagementService.CreateRegistrantKey(importedOrder.FirstName,
                        importedOrder.LastName, importedOrder.BillingEmail, orderRow.idWebinar,
                        orderRow.Webinar.WebinarKey);
                    //"{\"registrantKey\":106033865,\"joinUrl\":\"https://www2.gotomeeting.com/join/739905466/106033865\"}"
                    //http://stackoverflow.com/questions/13588185/deserialize-json-string-using-json-net

                    if (ReferenceEquals(null, regKeyResponse))
                        throw new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response.");

                    JObject parsedJsonObject = JObject.Parse(regKeyResponse);

                    if (parsedJsonObject["registrantKey"] != null)
                    {
                        var registrantKey = parsedJsonObject["registrantKey"].ToString();
                        var joinUrl = parsedJsonObject["joinUrl"].ToString();

                        orderRow.RegistrantKey = registrantKey;
                        orderRow.JoinURL = joinUrl;
                    }
                    else
                    {
                        /*  *************** 404 error condition *************** 
                         * json payload will look like:
                         *      {"description":"The webinar does not exist.","incident":3984078431536134144}
                         * which is not usable
                         */
                    }
                }

                //orderRow.RegistrantKey = "SomeKey";
                //orderRow.JoinURL = "https://www2.gotomeeting.com/join/739905466/106033865";
                //_orderManagementService.SaveOrderChanges(importedOrder);
                _orderManagementService.SaveOrderChanges(importedOrder, verificationKey, confirmChangeEmailUrl);

                idOfLastOrder = importedOrder.idOrder;
                _logger.Info("Posted idOrder=" + idOfLastOrder);
                //}

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.Created,
                    new { Result = idOfLastOrder.ToString() });
                httpResponseMessage.Headers.Location =
                    new Uri(Path.Combine(Request.RequestUri.ToString(), idOfLastOrder.ToString()));

            }
            catch (Exception exception)
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError,
                                    new { Result = "Not Saved" });
                // _logger.Fatal(exception.Message);
            }
            return httpResponseMessage;

        }

        // PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

    }
}