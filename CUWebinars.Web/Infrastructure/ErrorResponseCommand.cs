using System;
using CUWebinars.Web.Helpers;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace CUWebinars.Web.Infrastructure
{
    public class ErrorResponseCommand : IErrorResponseCommand
    {
        static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Execute(ErrorResponse errorResponse)
        {
            errorResponse.HttpContext.ClearError();
            errorResponse.HttpContext.Response.Clear();

            if (errorResponse.ExceptionInstance != null)
            {
                var originalRouteData =
                    System.Web.Routing.RouteTable.Routes.GetRouteData(new HttpContextWrapper(errorResponse.HttpContext));

                if (originalRouteData != null)
                {

                    string controllerNameOfOriginalRequest = GetRoutePart(WebUiConstants.Controller, originalRouteData);

                    //logger.Error(string.Format("Controller of Original Request: {0}", controllerNameOfOriginalRequest));

                    string actionNameOfOriginalRequest = GetRoutePart(WebUiConstants.Action,
                        originalRouteData
                        );

                    logger.Error(string.Format("ErrorResponseCMD logs Controller: {0} | Action: {1}"
                        , controllerNameOfOriginalRequest
                        , actionNameOfOriginalRequest));

                    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(string.Format("error in: {1} | {0} ", actionNameOfOriginalRequest, controllerNameOfOriginalRequest)));
                    if (!ReferenceEquals(actionNameOfOriginalRequest, null))
                    {
                        errorResponse.Controller.ViewData.Model = new HandleErrorInfo(errorResponse.ExceptionInstance,
                            controllerNameOfOriginalRequest,
                            actionNameOfOriginalRequest
                            );
                    }
                    else
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(string.Format("ErrorResponseCommand finds Null Controller {1} | {0} ", actionNameOfOriginalRequest, controllerNameOfOriginalRequest)));
                    }
                }
 
            }

            // Re-route execution to the relevant StaticContentController's Action method based on status code.
            if (!ReferenceEquals(errorResponse.Controller, null))
                if (errorResponse.NewrouteData != null)
                    ((IController)errorResponse.Controller).Execute(
                        new System.Web.Routing.RequestContext(
                            new HttpContextWrapper(errorResponse.HttpContext),
                            errorResponse.NewrouteData
                            ));
        }

        private string GetRoutePart(string routePart, System.Web.Routing.RouteData routeData)
        {
            string part = string.Empty;

            var value = routeData.Values[routePart];

            if (value != null)
                part = value.ToString();

            return part;
        }
    }
}