using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class GetGridDataAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            switch (action.Value)
            {
                case IdentityConstants.Manage:
                    {
                        return null;
                    }
                case IdentityConstants.Access:
                    {
                        return GetGridDataFeature;
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format(IdentityConstants.Invalid, action, "action"));
                    }
            }
        }

        public bool GetGridDataFeature(ClaimsPrincipal claimsPrincipal)
        {
            var result = claimsPrincipal.HasClaim(c => c.Type == Business.Constants.ClaimTypes.Affiliate || c.Type == Business.Constants.ClaimTypes.Admin);
            return result;
        }

    }
}