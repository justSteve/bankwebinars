using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class AdminControllerAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            return CanAccessAdminControllerActions;
        }
        public bool CanAccessAdminControllerActions(ClaimsPrincipal claimsPrincipal)
        {
            // all we are checking in the predicate is whether the principal has an Admin Claim. We don't care what value it has.
            return claimsPrincipal.HasClaim(c => c.Type == Business.Constants.ClaimTypes.Admin);
        }
    }
}