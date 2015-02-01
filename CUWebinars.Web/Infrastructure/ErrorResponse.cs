using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Web.Infrastructure
{
    public class ErrorResponse
    {
        public Controller Controller { get; set; }
        public Exception ExceptionInstance { get; set; }
        public HttpContext HttpContext { get; set; }
        public RouteData NewrouteData { get; set; }
    }
}