using CUWebinars.Web.Helpers;
using System.Web;
using System.Web.Mvc;

namespace CUWebinars.Web.Infrastructure
{
    public class ErrorResponseCommand : IErrorResponseCommand
    {
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
                    string controllerNameOfOriginalRequest = GetRoutePart(WebUiConstants.Controller,
                        originalRouteData
                        );

                    string actionNameOfOriginalRequest = GetRoutePart(WebUiConstants.Action,
                        originalRouteData
                        );
                    errorResponse.Controller.ViewData.Model = new HandleErrorInfo(errorResponse.ExceptionInstance,
                        controllerNameOfOriginalRequest,
                        actionNameOfOriginalRequest
                        );
                }
            }

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