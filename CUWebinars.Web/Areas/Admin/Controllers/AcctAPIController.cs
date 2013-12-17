using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
//using CUWebinars.Data.Repositories;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;


namespace CUWebinars.Web.Areas.Admin.Controllers
{

    public class AcctAPIController : ApiController
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        //private readonly IAccountRepository  _repos;
        public ILogger Logger { get; set; }

        public IMembershipService membershipService;
        
        public AcctAPIController(IMailService mail, ILogger logger, IMembershipService membershipService)
        {
            _mail = mail;
            //_repos = repos;
            Logger = logger;
            this.membershipService = membershipService;
        }
        // GET api/acctapi
        public HttpResponseMessage Get([FromUri] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user

                WebUserRepository repo = new WebUserRepository();
                
            

                var myInstitution = membershipService.ProcessInstitutionForUser(model.Institution,
                    model.BillingAddress.City,
                    model.BillingAddress.State,
                    "N",
                    "New",
                    model.BillingAddress.Zip);

                if (model.idWebUser == null)
                {
                    model.idWebUser= repo.FindHighestUserId();
                }

                try
                {
                    var address = new Address();
                    address.AddressType = "Billing";
                    address.City = model.BillingAddress.City;
                    address.Country = model.BillingAddress.Country;
                    address.Name = model.FirstName + ' ' + model.LastName;
                    address.Phone = model.BillingAddress.Phone;
                    address.State = model.BillingAddress.State;
                    address.StreetAddress = model.BillingAddress.StreetAddress;
                    address.StreetAddress2 = model.BillingAddress.StreetAddress2;
                    address.Zip = model.BillingAddress.Zip;

                    IList<Address> addresses = new List<Address> { address };

                    var result = membershipService.CreateUser(model.FirstName
                        , model.LastName
                        , model.FirstName + ' ' + model.LastName
                        , model.LastName.ToLower()
                        , model.Email
                        , USTimeZone.Central
                        , model.UserType
                        , myInstitution.idInstitution
                        
                        , addresses
                        , model.Title
                        , model.idWebUser
                        , "A");

                    HttpResponseMessage response1 = Request.CreateResponse(HttpStatusCode.Created, model);
                    response1.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = 19 }));
                    return response1;
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", e.StatusCode.ToString());
                }
            }

            // If we got this far, something failed, redisplay form
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, model);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = 19 }));
            return response;
        }

        // GET api/acctapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/acctapi
        public void Post([FromBody]string value)
        {
            var myVal = value;
            Logger.Info(myVal);
        }

        // PUT api/acctapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/acctapi/5
        public void Delete(int id)
        {
        }
    }
}
