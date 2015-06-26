using System;
using System.Security.Claims;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class ImpersonateFeatureAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            return CanAccessImpersonateFeature;
        }

        public bool CanAccessImpersonateFeature(ClaimsPrincipal claimsPrincipal)
        {
            // possibly check another claim targeted at impersonation.
            return claimsPrincipal.HasClaim(c => c.Type == Business.Constants.ClaimTypes.Affiliate || c.Type == Business.Constants.ClaimTypes.Admin);
        }
    }
}