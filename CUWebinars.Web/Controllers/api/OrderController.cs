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
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Controllers.api
{
    public class OrderController : ApiController
    {
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;

        public OrderController(IMembershipService membershipService, IOrderManagementService orderManagementService)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
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
            var webinar = _orderManagementService.GetWebinar(model.idWebinar);
            var webUser = _membershipService.GetUserByEmail(model.Email);
            var option = _orderManagementService.GetOptionById(model.idOption);
            var affiliate = _orderManagementService.GetAffiliateById(model.idAffiliate);
            model.BillingAddress.WebUser = webUser;
            model.ShippingAddress.WebUser = webUser;
            var additionalLocationsAsString = string.Join(";", model.AdditionalLocations);

            OrderRowOption orderRowOption = null;

            if (model.AdditionalLocations.Any())
            {
                orderRowOption = _orderManagementService.CreateOrderRowOption(option,
                    option.OptionExplain,
                    Convert.ToDecimal(option.PriceToAdd ?? 0.0),
                    additionalLocationsAsString,
                    model.AdditionalLocations.Count,
                    model.AdditionalLocations.ToArray()
                    );
            }

            var orderRow = _orderManagementService.CreateOrderRow(
                webinar,
                orderRowOption,
                additionalLocationsAsString,
                model.idOption
                );
            orderRow.idDiscount = model.Discount;
            orderRow.Status = model.Status;

            var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

            importedOrder.AdminComments = model.AdminComments;
            importedOrder.AffiliateComments = model.AffiliateComments;
            importedOrder.UserComments = model.UserComments;
            importedOrder.Origin = model.Origin;
            importedOrder.BillingAddress = model.BillingAddress.StreetAddress;
            importedOrder.BillingAddress2 = model.BillingAddress.StreetAddress2;
            importedOrder.BillingPhone = model.BillingAddress.Phone;
            importedOrder.BillingEmail = model.Email;
            importedOrder.BillingCity= model.BillingAddress.City;
            importedOrder.BillingState = model.BillingAddress.State;
            importedOrder.BillingZip= model.BillingAddress.Zip;
            importedOrder.FirstName = webUser.FirstName;
            importedOrder.LastName = webUser.LastName;
            importedOrder.Institution = webUser.Institution.InstitutionName;
            
            importedOrder.ShippingAddress = model.ShippingAddress.StreetAddress;
            importedOrder.ShippingAddress2 = model.ShippingAddress.StreetAddress2;
            importedOrder.ShippingPhone = model.ShippingAddress.Phone;
            importedOrder.ShippingCity= model.ShippingAddress.City;
            importedOrder.ShippingState = model.ShippingAddress.State;
            importedOrder.ShippingZip= model.ShippingAddress.Zip;
            importedOrder.ShippingFirstName = webUser.FirstName;
            importedOrder.ShippingLastName = webUser.LastName;

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