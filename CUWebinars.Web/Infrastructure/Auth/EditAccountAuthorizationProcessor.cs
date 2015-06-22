using CUWebinars.Web.Helpers;
using System;
using System.Security.Claims;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public class EditAccountAuthorizationProcessor : IAuthorizationProcessor
    {
        public Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action)
        {
            switch (action.Value)
            {
                case IdentityConstants.Manage:
                    {
                        return CanManageEditUserFeature;
                    }
                case IdentityConstants.Access:
                {
                    return CanManageEditUserFeature;
                }
                default:
                    {
                        throw new NotSupportedException(string.Format(IdentityConstants.Invalid, action, "action"));
                    }
            }
        }

        public bool CanManageEditUserFeature(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }
    }
}