using Elmah;
using System.Web.Mvc;

namespace CUWebinars.Web.Infrastructure.Attributes
{
    public class HandleAjaxExceptionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
            {
                LogException(filterContext);

                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;

                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        Message = "Uncaught Ajax Error"
                    }
                };
                filterContext.ExceptionHandled = true;
            }
        }

        private static void LogException(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext.ApplicationInstance.Context;
            var error = new Error(context.Exception, httpContext);
            ErrorLog.GetDefault(httpContext).Log(error);
        }
    }
}