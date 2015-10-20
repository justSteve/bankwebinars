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
                "PostBackWPS",
                url: "PostBackWPS",
                defaults: new { controller = "Cart", action = "PostBackWPS", id = 0 }
            );
            routes.MapRoute(
                "expresscheckout",
                url: "expresscheckout",
                defaults: new { controller = "Admin", action = "ExpressCheckout", id = 0 }
            );
            routes.MapRoute(
                "blog",
                url: "blog/",
                defaults: new { controller = "Admin", action = "RedirectToLegacyBlog"}
            );

            routes.MapRoute(
                "ccpostback",
                url: "ccpostback",
                defaults: new { controller = "Cart", action = "PostBackMoneris", id = 0 }
            );

            routes.MapRoute(
                "ccpostbackbw",
                url: "ccpostbackbw",
                defaults: new { controller = "Cart", action = "PostBackMonerisBW", id = 0 }
            );

            routes.MapRoute(
                "EditWebinar",
                url: "{id}/editWebinar",
                defaults: new { controller = "Webinar", action = "EditWebinarFromDetails", id = 0 },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                "EditQuiz",
                url: "{webinarId}/editquiz",
                defaults: new { controller = "Quiz", action = "EditQuizFromDetails", webinarId = 0 },
                constraints: new { webinarId = @"\d+" }
            );

            routes.MapRoute(
                "OnDemand",
                url: "o/{onDemandCode}",
                defaults: new { controller = "Webinar", action = "OnDemand" }

            );
            routes.MapRoute(
                "RedirectLegacy",
                url: "Webinar/Play",
                defaults: new { controller = "Webinar", action = "RedirectLegacy" }

            );

            routes.MapRoute(
                "RedirectLegacyHandouts",
                url: "handouts/{arg1}/{arg2}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyHandouts" }
            );
            routes.MapRoute(
                "RedirectLegacyHandouts1",
                url: "handouts/{arg1}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyHandouts" }
            );


            routes.MapRoute(
                "OnDemandPlaybackLegacy",
                url: "Webinar/OnDemandPlayback/{args}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyRecordings" }
            );

            
            routes.MapRoute(
                "DirSeriesCerts",
                url: "Admin/registrations/CertificateOfCompletionDS",
                defaults: new { controller = "Admin", action = "CertificateOfCompletionDS" }
            );

                  
            routes.MapRoute(
                "Recorded",
                url: "Webinar/Recorded",
                defaults: new { controller = "Webinar", action = "Recorded" }
            );

                  
            routes.MapRoute(
                "Upcoming",
                url: "Webinar/Upcoming",
                defaults: new { controller = "Webinar", action = "Upcoming" }
            );

            routes.MapRoute(
                "RedirectLegacyRecordings",
                url: "Recordings/{arg1}/{arg2}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyRecordings" }
            );
            routes.MapRoute(
                "RedirectLegacyRecordings1",
                url: "Recordings/{arg1}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyRecordings" }
            );



            routes.MapRoute(
                "Quiz",
                url: "Quiz/Index/{quizCode}",
                defaults: new { controller = "Quiz", action = "Index" }
            );

            routes.MapRoute(
                "ClickToJoin",
                url: "j/{joinCode}",
                defaults: new { controller = "Webinar", action = "ClickToJoin", joinCode = string.Empty }
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
                "AddPasswordFromUserEmail",
                url: "acc/apwd/{email}",
                defaults: new { controller = "Account", action = "AddPasswordForCartCreatedUser", email = "" }
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
                constraints: new { webinarId = @"\d+" }
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
