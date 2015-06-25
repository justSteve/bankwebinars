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
                    //return PrincipalCanPerformActionOnResource(action, context.Principal);
                    return false;
                }
                case IdentityConstants.admin:
                case IdentityConstants.Admin:
                {
                    authorizationProcessor = new AdminControllerAuthorizationProcessor();
                    return ProcessAuthorizationRequest(context, authorizationProcessor, action);
                }
                case IdentityConstants.AdminFunction:
                {
                    authorizationProcessor = new AdminFunctionAuthorizationProcessor();
                    return ProcessAuthorizationRequest(context, authorizationProcessor, action);
                }
                case IdentityConstants.BatchPasswordResetFeature:
                {
                    authorizationProcessor = new BatchPasswordResetAuthorizationProcessor();
                    return ProcessAuthorizationRequest(context, authorizationProcessor, action);
                }
                default:
                {
                    throw new NotSupportedException(string.Format(IdentityConstants.Invalid, resource, "resource"));
                }
            }
        }

        private bool ProcessAuthorizationRequest(
            AuthorizationContext context,
            IAuthorizationProcessor authorizationProcessor, 
            Claim action)
        {
            var processor = authorizationProcessor.GetAuthorizationProcessorForAction(action);
            return processor(context.Principal);
        }
    }
}