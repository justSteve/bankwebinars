using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers.api
{
    public class OrderController : ApiController
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;
        private readonly ILogger _logger;

        public OrderController(IMembershipService membershipService, IOrderManagementService orderManagementService, IStateService stateService, ILogger logger)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _logger = logger;
        }

        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}
        //   

        //http://localhost:5556/api/order
        //Host: localhost:5556
        //Content-Type: application/json; charset=utf-8
        //Connection: keep-alive
        //Accept: application/json, text/javascript, */*; q=0.01:
        //Content-Length: 1059

        /*
        {
          "AdditionalLocations": [    
            "dave@turing.com",
            "jed@turing.com"
            ],
          "AffiliateComments": "Affiliate comments",
          "BillingAddress": {    
            "AddressType": "Billing",
            "Name": "Alan Turing",
            "Phone": "555-555-5555",
            "StreetAddress": "968 Wildcat Dr",
            "StreetAddress2": "",
            "City": "Del Rio",
            "Zip": "5000",
            "State": "Tx",
            "Country": "USA",    
          },
          "Email": "alanbturingt@turing.com",
          "FirstName": "Alan",
          "LastName": "Turing",
          "idAffiliate": 19,
          "idRegType": 88,  
          "idWebinar": 437,
          "Institution": "Some Institution",
          "ShippingAddress": {    
            "AddressType": "Shipping",
            "Name": "Alan Turing",
            "Phone": "555-555-5555",
            "StreetAddress": "968 Wildcat Dr",
            "StreetAddress2": "",
            "City": "Del Rio",
            "Zip": "5000",
            "State": "Tx",
            "Country": "USA",    
          },
          "SendNotification": "true",
          "Title": "Mr",
        }
         * 
                 * 
                 * */
        // POST api/<controller>
        public HttpResponseMessage Post([FromBody]IncomingOrderModel model)
        {
            var email = model.Email.Trim();
            var firstName = model.FirstName.Trim();
            var lastName = model.LastName.Trim();

            var webinar = _orderManagementService.GetWebinar(model.idWebinar);
            var webUser = _membershipService.GetUserByEmail(email);
            var affiliate = _orderManagementService.GetAffiliateById(model.idAffiliate);

            if (webUser == null)
            {
                var institutionForUser = _membershipService.ProcessInstitutionForUser(model.Institution.Trim(),
                    model.Email,
                    model.BillingAddress.City,
                    model.BillingAddress.State,
                    "N",
                    "New",
                    model.BillingAddress.Zip
                    );

                IList<Address> addresses = new List<Address> { model.BillingAddress, model.ShippingAddress };

                USTimeZone userTimeZone = _membershipService.GetTimeZoneByZip();
                webUser = _membershipService.CreateWebUser(globalConfig.Tenant
                    , firstName
                    , lastName
                    , lastName.ToLower().Trim()
                    , email
                    , userTimeZone
                    , UserType.Customer
                    , institutionForUser.idInstitution
                    , addresses
                    , model.Title == null ? model.Title : model.Title.Trim()
                    , null
                    , "A"
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


            if (model.AdditionalLocation != null && model.AdditionalLocation.Any())
            {
                var price = 150;
                //string email,decimal price,string fullname
                var makeAddLocs = model.AdditionalLocation.ToString().Split(',');
                var i = 0;
                foreach (var makeAddLoc in makeAddLocs)
                {
                    var AEmail = makeAddLocs[i];
                    var AFirstName = email.Split('@')[0].ToString();
                    var ALastName = email.Split('@')[1].ToString();
                    addLocation.Add(_orderManagementService.CreateAdditionalLocation(AEmail, price, AFirstName + ' ' + ALastName));
                    i++;
                }
            }

            var orderRow = _orderManagementService.CreateOrderRow(
                webinar,
                addLocation,
                model.idRegType
                );

            orderRow.Discount = _orderManagementService.GetDiscount(model.Email);

            var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

            importedOrder.AdminComments = "model.AdminComments";
            importedOrder.AffiliateComments = model.AffiliateComments;
            importedOrder.UserComments = "model.UserComments";
            importedOrder.Origin = "model.Origin";
            importedOrder.FirstName = webUser.FirstName;
            importedOrder.LastName = webUser.LastName;
            importedOrder.Institution = webUser.Institution.InstitutionName;

            importedOrder.BillingAddress = model.BillingAddress.StreetAddress;
            importedOrder.BillingAddress2 = model.BillingAddress.StreetAddress2;
            importedOrder.BillingPhone = model.BillingAddress.Phone;
            importedOrder.BillingEmail = model.Email;
            importedOrder.BillingCity = model.BillingAddress.City;
            importedOrder.BillingState = model.BillingAddress.State;
            importedOrder.BillingZip = model.BillingAddress.Zip;

            importedOrder.ShippingAddress = model.ShippingAddress.StreetAddress;
            importedOrder.ShippingAddress2 = model.ShippingAddress.StreetAddress2;
            importedOrder.ShippingPhone = model.ShippingAddress.Phone;
            importedOrder.ShippingCity = model.ShippingAddress.City;
            importedOrder.ShippingState = model.ShippingAddress.State;
            importedOrder.ShippingZip = model.ShippingAddress.Zip;
            importedOrder.ShippingFirstName = firstName;
            importedOrder.ShippingLastName = lastName;

            if (orderRow.Webinar.WebinarKey > 0)
            {
                var regKeyResponse = _orderManagementService.CreateRegistrantKey(importedOrder.FirstName, importedOrder.LastName, importedOrder.BillingEmail, orderRow.idWebinar, orderRow.Webinar.WebinarKey);
                //"{\"registrantKey\":106033865,\"joinUrl\":\"https://www2.gotomeeting.com/join/739905466/106033865\"}"
                //http://stackoverflow.com/questions/13588185/deserialize-json-string-using-json-net

                if (regKeyResponse.Contains("registrantKey"))
                {
                    var regKey = regKeyResponse.Split(',')[0].Split(':')[1];
                    orderRow.RegistrantKey = regKey;
                }
                if (regKeyResponse.Contains("joinUrl"))
                {
                    var joinURL = regKeyResponse.Split(',')[0].Split(':')[1];
                    orderRow.JoinURL = joinURL;
                }

            }

            _orderManagementService.SaveOrderChanges(importedOrder);

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.Created, new { Result = "Order successfully submitted" });
            httpResponseMessage.Headers.Location = new Uri(Path.Combine(Request.RequestUri.ToString(), importedOrder.idOrder.ToString()));

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