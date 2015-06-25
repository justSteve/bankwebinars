using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class AdminFunctionAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            switch (action.Value)
            {
                case IdentityConstants.Manage:
                    {
                        return CanAccessAdminFeature;
                    }
                case IdentityConstants.Access:
                    {
                        return CanAccessAdminFeature;
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format(IdentityConstants.Invalid, action.Value, "action"));
                    }
            }
        }

        public bool CanAccessAdminFeature(ClaimsPrincipal claimsPrincipal)
        {
            // all we are checking in the predicate is whether the principal has an Admin Claim. We don't care what value it has.
            return claimsPrincipal.HasClaim(c => c.Type == Business.Constants.ClaimTypes.Admin);
        }
    }
}