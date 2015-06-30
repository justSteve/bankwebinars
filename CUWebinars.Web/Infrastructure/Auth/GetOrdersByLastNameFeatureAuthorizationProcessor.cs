using System;
using System.Security.Claims;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class GetOrdersByLastNameFeatureAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            return CanAccessGetOrdersByLastNameFeature;
        }

        public bool CanAccessGetOrdersByLastNameFeature(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(c => c.Type == Business.Constants.ClaimTypes.Affiliate || c.Type == Business.Constants.ClaimTypes.Admin);
        }
    }
}