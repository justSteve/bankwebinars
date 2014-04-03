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
            var orderRow = _orderManagementService.CreateOrderRow(
                webinar, 
                null, // OrderRowOption gord here 
                model.AlternativeEmail,
                model.idOption
                );

            var importedOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);

            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.Created, new { Result = "Order successfully submitted"});
            //httpResponseMessage.Headers.Location = new Uri(Path.Combine(Request.RequestUri.ToString(), importedOrder.idOrder.ToString()));

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