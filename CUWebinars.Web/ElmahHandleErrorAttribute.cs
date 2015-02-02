using Elmah;
using System.Web.Mvc;

namespace CUWebinars.Web
{
    public class ElmahHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var e = context.Exception;
            if (!context.ExceptionHandled   // if unhandled, will be logged anyhow
                || RaiseErrorSignal(context)      // prefer signaling, if possible
                || IsFiltered(context))     // filtered?
                return;

            LogException(context);
        }

        private static bool RaiseErrorSignal(ExceptionContext context)
        {
            var httpContext = context.HttpContext.ApplicationInstance.Context;

            if (httpContext == null)
                return false;

            var signal = ErrorSignal.FromContext(httpContext);

            if (signal == null)
                return false;

            signal.Raise(context.Exception, httpContext);
            return true;
        }

        private static bool IsFiltered(ExceptionContext context)
        {
            var config = context.HttpContext.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;

            if (config == null)
                return false;

            var testContext = new ErrorFilterModule.AssertionHelperContext(
                                      context.Exception, context.HttpContext.ApplicationInstance.Context
                                      );

            return config.Assertion.Test(testContext);
        }

        private static void LogException(ExceptionContext context)
        {
            var httpContext = context.HttpContext.ApplicationInstance.Context;
            var error = new Error(context.Exception, httpContext);
            ErrorLog.GetDefault(httpContext).Log(error);
        }
    }
}
