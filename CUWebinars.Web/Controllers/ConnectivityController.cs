using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class ConnectivityController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;

        public ConnectivityController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        // GET: Connectivity
        public ActionResult Index()
        {
            return View((object)_globalConfig.TenantURL);
        }
        public ActionResult Result(string id, string url)
        {
            var success = (id == "ok");
            if (User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                var newClaimType = "connectivity_to_" + url;
                var newClaimValue = success ? "yes" : "no";
                if (!identity.HasClaim(newClaimType, newClaimValue))
                {
                    var emailClaim = identity.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                    if (emailClaim != null)
                    {
                        var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, emailClaim.Value);
                        if (userAccount != null)
                        {
                            _membershipService.AddClaim(userAccount, newClaimType, newClaimValue);
                        }
                    }
                }
            }
            return null;
        }
    }
}