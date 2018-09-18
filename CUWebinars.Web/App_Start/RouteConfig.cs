using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes(); // enable attribute routing
            routes.MapRoute(
                name: "php",
                url: "{page}.php",
                defaults: new { controller = "Home", action = "Index", page = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "blog",
                url: "blog",
                defaults: new { controller = "Home", action = "Index", page = UrlParameter.Optional }
            );
            //replicates legacy's generic connection info endpoint
            routes.MapRoute(
                    "WebinarDetails1",
                    "{id}",
                    new { controller = "Webinar", action = "details", id = "" },  // Parameter defaults
                    new { id = @"\d+" }
                );

            routes.MapRoute(
                "PostBackWPS",
                url: "PostBackWPS",
                defaults: new { controller = "Cart", action = "PostBackWPS", id = 0 }
            );
            //routes.MapRoute(
            //    "wp-login.php",
            //    url: "wp-login.php",
            //    defaults: new { controller = "Home", action = "index", id = 0 }
            //);
            routes.MapRoute(
                "Incoming",
                url: "Incoming",
                defaults: new { controller = "Cart", action = "Incoming" }
            );
            routes.MapRoute(
                name: "ResumeCheckout",
                url: "Resume/{id}",
               defaults: new { controller = "Cart", action = "Resume", id = 0 }
            );

            routes.MapRoute(
                name: "WSP",
                url: "WSP",
               defaults: new { controller = "Webinar", action = "Details", id = 2520 }
            );
            if (GlobalConfig.GlobalConfigSingleton.Tenant == "CUWebinars")
            {

            }

            if (GlobalConfig.GlobalConfigSingleton.Tenant == "BankWebinars")
            {
                routes.MapRoute(
                    name: "topicDeposit",
                    url: "DepositAccounts",
                   defaults: new { controller = "Webinar", action = "ListByTopic", id = 31 }
                );

                routes.MapRoute(
                    name: "topicManagementEmployeeDevelopment",
                    url: "DepositAccounts",
                    defaults: new { controller = "Webinar", action = "ListByTopic", id = 31 }
                );

            }
            routes.MapRoute(
                name: "UpdateAffiliate",
                url: "UpdateAffiliate",
               defaults: new { controller = "Cart", action = "UpdateAffiliate", id = 0 }

            );
            routes.MapRoute(
                "expresscheckout",
                url: "expresscheckout",
                // this should work and would be better
                // id to the method 
                //defaults: new { controller = "Admin", action = "ExpressCheckoutPostBack", id = 0 }
                defaults: new { controller = "Admin", action = "ExpressCheckout", id = 0 }
            );
            routes.MapRoute(
                "express",
                url: "express",
                defaults: new { controller = "Cart", action = "Express", id = 0 });

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
                url: "Recordings/{recordingURL}",
                defaults: new { controller = "Webinar", action = "RedirectLegacyRecordings" }
            );

            routes.MapRoute(
                "RedirectLegacyHandouts",
                url: "handouts/",
                defaults: new { controller = "Webinar", action = "RedirectLegacyHandouts" }
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
                new { controller = "Cart", action = string.Empty, id = string.Empty }, // Parameter defaults
                new { action = "MyWebinars" }
            );

            //routes.MapRoute(
            //    "MyWebinarsDefault",                                    // Route name
            //    "{action}/{id}",                                        // URL with parameters
            //    new { controller = "Account", action = string.Empty, id = string.Empty }, // Parameter defaults
            //    new { action = "MyWebinars" }
            //);

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

        }
    }
}
