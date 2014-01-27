using CUWebinars.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace CUWebinars.Web.Infrastructure
{
    public class TTSAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var resource = context.Resource.First().Value;
            var action = context.Action.First();

            switch (resource)
            {
                case IdentityConstants.Account:
                    {
                        return PrincipalCanPerformActionOnResource(action, context.Principal);
                    }
                case IdentityConstants.AdminResources:
                    {
                        if (PrincipalCanPerformActionOnResource(action, context.Principal))
                            return true;
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format(IdentityConstants.Invalid, resource, "resource"));
                    }
            }

            return false;
        }

        private bool PrincipalCanPerformActionOnResource(Claim action, ClaimsPrincipal principal)
        {
            switch (action.Value)
            {
                case IdentityConstants.Manage:
                    {
                        if (principal.HasClaim(ClaimTypes.Role, IdentityConstants.Admin))
                        {
                            return true;
                        }
                        break;
                    }

                case IdentityConstants.Access:
                    {
                        if (principal.HasClaim(ClaimTypes.Role, IdentityConstants.Admin))
                        {
                            return true;
                        }
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format(IdentityConstants.Invalid, action, "action"));
                    }
            }

            return false;
        }
    }
}