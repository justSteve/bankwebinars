using System;
using System.Security.Claims;

namespace CUWebinars.Web.Infrastructure.Auth
{
    public interface IAuthorizationProcessor
    {
        Func<ClaimsPrincipal, bool> GetAuthorizationProcessorForAction(Claim action);
    }
}