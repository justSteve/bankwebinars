using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Auth;
using System;
using System.Linq;
using System.Security.Claims;

namespace CUWebinars.Web.Infrastructure
{
    // ReSharper disable once InconsistentNaming
    public class TTSAuthorizationManager : ClaimsAuthorizationManager
    {
        public override bool CheckAccess(AuthorizationContext context)
        {
            var resource = context.Resource.First().Value;
            var action = context.Action.First();

            IAuthorizationProcessor authorizationProcessor;

            switch (resource)
            {
                case IdentityConstants.Account:
                {
                    authorizationProcessor = new EditAccountAuthorizationProcessor();

                    return authorizationProcessor.GetAuthorizationProcessorForAction(action)(context.Principal);
                }
                case IdentityConstants.AdminResources:
                {
                    //if (PrincipalCanPerformActionOnResource(action, context.Principal))
                    //    return true;
                    return false;
                }
                case IdentityConstants.BatchPasswordResetFeature:
                {
                    authorizationProcessor = new BatchPasswordResetAuthorizationProcessor();
                    var processor = authorizationProcessor.GetAuthorizationProcessorForAction(action);
                    return processor(context.Principal);
                }
                default:
                {
                    throw new NotSupportedException(string.Format(IdentityConstants.Invalid, resource, "resource"));
                }
            }
        }
    }
}