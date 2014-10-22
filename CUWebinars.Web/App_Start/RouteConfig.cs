using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    "Admin",                                                    // Route name
            //    "{controller}",                        // URL with parameters
            //    new { controller = "Home", action = "Index" }

            //);
            routes.MapRoute(
                "MyWebinarsDefault",                                    // Route name
                "{action}/{id}",                                        // URL with parameters
                new { controller = "Account", action = "", id = "" }, // Parameter defaults
                new { action = "MyWebinars" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "WebinarPlusWebUser",
                url: "{controller}/{action}/{webinarId}/{webUserId}"
            );

            routes.MapRoute(
                name: "EmailLinkRoute",
                url: "{controller}/{action}/{email}/{surname}"
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
