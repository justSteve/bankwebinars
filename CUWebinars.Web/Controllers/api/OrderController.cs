using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;

namespace CUWebinars.Web.Controllers.api
{
    public class OrderController : ApiController
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;

        public OrderController(IMembershipService membershipService, IOrderManagementService orderManagementService, IStateService stateService)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
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

                webUser = _membershipService.CreateWebUser(globalConfig.Tenant
                    , firstName
                    , lastName
                    , model.Password.Trim()
                    , email
                    , model.UsTimeZone
                    , model.UserType
                    , institutionForUser.idInstitution
                    , addresses
                    , model.Title == null ? model.Title : model.Title.Trim()
                    , null
                    , "A"
                    );

                
                if (!_stateService.HasValue(Constants.CurrentUser))
                    _stateService.SetValue<WebUser>(Constants.CurrentUser, webUser); //  gets retrieved in TTSTokenizer


                _membershipService.CreateUser(globalConfig.Tenant
                    , firstName
                    , lastName
                    , email
                    , model.Password
                    , email
                    );
            }

            AdditionalLocations additionalLocations = null;

            //if (model.AdditionalLocations.Any())
            //{
            //    var optionsForWebinar = _orderManagementService.GetOptionsByWebinarId(webinar.idWebinar, false);
            //    var optionForAdditionalLocation = optionsForWebinar.First(o => o.Type == "additional_location");

            //    additionalLocations = _orderManagementService.CreateOrderRowOption(optionForAdditionalLocation,
            //        optionForAdditionalLocation.OptionExplain,
            //        Convert.ToDecimal(optionForAdditionalLocation.Price ?? 0.0),
            //        model.AdditionalLocations.ToArray()
            //        );
            //}

            var orderRow = _orderManagementService.CreateOrderRow(
                webinar,
                additionalLocations,
                model.AlternativeEmail,
                model.idRegType
                );

            orderRow.Discount = model.Discount;
            orderRow.RowStatus = model.Status;

            var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

            importedOrder.AdminComments = model.AdminComments;
            importedOrder.AffiliateComments = model.AffiliateComments;
            importedOrder.UserComments = model.UserComments;
            importedOrder.Origin = model.Origin;
            importedOrder.FirstName = webUser.FirstName;
            importedOrder.LastName = webUser.LastName;
            importedOrder.Institution = webUser.Institution.InstitutionName;

            importedOrder.BillingAddress = model.BillingAddress.StreetAddress;
            importedOrder.BillingAddress2 = model.BillingAddress.StreetAddress2;
            importedOrder.BillingPhone = model.BillingAddress.Phone;
            importedOrder.BillingEmail = model.Email;
            importedOrder.BillingCity= model.BillingAddress.City;
            importedOrder.BillingState = model.BillingAddress.State;
            importedOrder.BillingZip= model.BillingAddress.Zip;
            
            importedOrder.ShippingAddress = model.ShippingAddress.StreetAddress;
            importedOrder.ShippingAddress2 = model.ShippingAddress.StreetAddress2;
            importedOrder.ShippingPhone = model.ShippingAddress.Phone;
            importedOrder.ShippingCity= model.ShippingAddress.City;
            importedOrder.ShippingState = model.ShippingAddress.State;
            importedOrder.ShippingZip= model.ShippingAddress.Zip;
            importedOrder.ShippingFirstName = firstName;
            importedOrder.ShippingLastName = lastName;

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