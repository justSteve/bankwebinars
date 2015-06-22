using System;
using System.Security.Claims;
using CUWebinars.Business.Constants;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class BatchPasswordResetAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            switch (action.Value)
            {
                case IdentityConstants.Manage:
                {
                    throw new NotImplementedException();
                }
                case IdentityConstants.Access:
                {
                    return CanAccessBatchPasswordResestFeature;
                }
                default:
                {
                    throw new NotSupportedException(string.Format(IdentityConstants.Invalid, action, "action"));
                }
            }
        }

        public bool CanAccessBatchPasswordResestFeature(ClaimsPrincipal claimsPrincipal)
        {
            switch (GlobalConfig.GlobalConfigSingleton.Tenant)
            {
                case DomainConstants.BankWebinars:
                    if (claimsPrincipal.HasClaim(Business.Constants.ClaimTypes.BWMigratorPasswordReset, "true"))
                    {
                        return true;
                    }
                    return false;
                case DomainConstants.CUWebinars:
                    if (claimsPrincipal.HasClaim(Business.Constants.ClaimTypes.CUMigratorPasswordReset, "true"))
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