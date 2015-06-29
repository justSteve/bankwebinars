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
            switch (GlobalConfig.GlobalConfigSingleton.Tenant)
            {
                case DomainConstants.BankWebinars:
                    if (claimsPrincipal.HasClaim(Business.Constants.ClaimTypes.Affiliate, "true"))
                    {
                        return true;
                    }
                    return false;
                case DomainConstants.CUWebinars:
                    if (claimsPrincipal.HasClaim(Business.Constants.ClaimTypes.Affiliate, "true"))
                    {
                        return true;
                    }
                    return false;
                default:
                    throw new NotSupportedException(string.Format("{0} is not a known tenant", GlobalConfig.GlobalConfigSingleton.Tenant));
            }
        }

    }
}