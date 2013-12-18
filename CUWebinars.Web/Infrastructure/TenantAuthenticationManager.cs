using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace CUWebinars.Web.Infrastructure
{
    public class TenantAuthenticationManager : ClaimsAuthenticationManager
    {
        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)incomingPrincipal.Identity;
                identity.AddClaim(new Claim(ClaimTypes.Name, identity.Name));
            }

            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}