using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "EditWebinar",
                url: "{id}/edit",
                defaults: new { controller = "Webinar", action = "EditWebinarFromDetails", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                "EditOrder",
                url: "Admin/ManageOrder/{id}",
                defaults: new { controller = "Admin", action = "ManageOrderFromDetails", id = 0 },
                constraints: new { id = @"^[1-9][0-9]*$" }
            );
            
            routes.MapRoute(
                "WebinarDetails",
                url: "{id}/{seoUrl}",
                defaults: new { controller = "Webinar", action = "Details", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                "MyWebinarsDefault",                                    // Route name
                "{action}/{id}",                                        // URL with parameters
                new { controller = "Account", action = string.Empty, id = string.Empty }, // Parameter defaults
                new { action = "MyWebinars" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "WebinarPlusWebUser",
                url: "{controller}/{action}/{webinarId}/{webUserId}",
                defaults: new { controller = "Cart", action = "GetAdditionalLocationByOrderId" },
                constraints: new {webinarId = @"\d+"}
            );

            routes.MapRoute(
                name: "EmailLinkRoute",
                url: "{controller}/{action}/{email}/{password}"
            );

            //  Catch-all, for any routes which do not exist.
            routes.MapRoute(
                "404PageNotFound",
                "{*url}",
                new { controller = WebUiConstants.StaticContent, action = WebUiConstants.PageNotFound }
                );

            //in the legacy system this route will catch:
            // bankwebinars.com/1522
            // and redirect to the webinarController.ConnInfo
            //routes.MapRoute(
            //        "WebinarConnectionDetails",                                             // Route name
            //        "{id}",                                                                 // URL with parameters
            //        new { controller = "Webinar", action = "ConnectionDetails", id = "" },  // Parameter defaults
            //        new{id = @"\d+"}
            //    );

            //add route that responds to
            // 1) cuwebinars.com/mywebinars
            // 2) mywebinars.cuwebinars.com

            // by redirecting to AccountController.NewMethodThatHandlesReturningUsers
        }
    }
}
