using System;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CUWebinars.Web.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (ReferenceEquals(filterContext, null)) throw new ArgumentNullException("filterContext");

            var request = filterContext.HttpContext.Request;

            //  Only validate POSTs
            if (request.HttpMethod == WebRequestMethods.Http.Post)
            {
                //  Ajax POSTs and normal form posts have to be treated differently when it comes
                //  to validating the AntiForgeryToken
                //if (request.IsAjaxRequest())
                //{
                //    var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];

                //    var cookieValue = ReferenceEquals(antiForgeryCookie, null) ? null : antiForgeryCookie.Value;

                //    AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                //}
                //else
                //{
                //    new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
                //}
            }

        }
    }
}