using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Data.Repositories;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Utils;
using log4net;
using System;
using System.IdentityModel.Claims;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CUWebinars.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static TTSWebinarsContext ctx = new TTSWebinarsContext();
        private static IAffiliateRepository _repos = new AffiliateRepository(ctx);
        private static IWebinarRepository _reposWebinars = new WebinarRepository(ctx);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Database.SetInitializer<TTSWebinarsContext>(new DatabaseInitializer());
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            log4net.Config.XmlConfigurator.Configure();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
        }
        
        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    var ex = Server.GetLastError().GetBaseException();

        //    var routeData = new RouteData();
        //    if (ex.GetType() == typeof(HttpException))
        //    {
        //        var httpException = (HttpException)ex;

        //        switch (httpException.GetHttpCode())
        //        {
        //            case 404:
        //                routeData.Values.Add("action", "PageNotFound");
        //                break;
        //            default:
        //                routeData.Values.Add("action", "GeneralError");
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        routeData.Values.Add("action", "GeneralError");
        //    }

        //    routeData.Values.Add("controller", "Error");
        //    routeData.Values.Add("error", ex);

        //    IController errorController = new ErrorController();
        //    errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));

        //}


        private void Session_Start(object sender, EventArgs e)
        {
            if (System.Web.HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != "~/account/get/")
            {
                Session["searchTerm"] = "";
                Session["IncludeRecorded"] = false;
                Session["IncludeUpcoming"] = true;
                Session["searchExtent"] = "Upcoming";


                CurrentSession.Instance.CurrentAffiliate = _repos.GetCurrentAffiliate();
                CurrentSession.Instance.AffiliateSessionSource = "default|" + AppConst.DEFAULT_AFFILIATE;
                CurrentSession.Instance.SubdomainBranding =
                    @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL];
                if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                    CurrentSession.Instance.FirstReferrer =
                        System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
                CurrentSession.Instance.FirstPage = System.Web.HttpContext.Current.Request.Url.ToString();
                CurrentSession.Instance.InitialQueryString = Request.QueryString;
                CurrentSession.Instance.SessionID = System.Web.HttpContext.Current.Session.SessionID;

                logger.Info("Start Session: " + CurrentSession.Instance.SessionID);

                var allCookies = new StringBuilder();
                for (var i = 0; i < Request.Cookies.Count; i++)
                {
                    HttpCookie aCookie = Request.Cookies[i];
                    if (aCookie != null)
                    {
                        allCookies.Append("Name = " + aCookie.Name + "<br />");
                        if (aCookie.HasKeys)
                        {
                            System.Collections.Specialized.NameValueCollection cookieValues =
                                aCookie.Values;
                            string[] cookieValueNames = cookieValues.AllKeys;
                            for (int j = 0; j < cookieValues.Count; j++)
                            {
                                string subkeyName = Server.HtmlEncode(cookieValueNames[j]);
                                string subkeyValue = Server.HtmlEncode(cookieValues[j]);
                                allCookies.Append("Subkey name = " + subkeyName + "<br />");
                                allCookies.Append("Subkey value = " + subkeyValue + "<br /><br />");
                            }
                        }
                        else
                        {
                            allCookies.Append("Value = " + Server.HtmlEncode(aCookie.Value) + "<br /><br />");
                        }
                    }
                }

                CurrentSession.Instance.FirstCookies = allCookies.ToString();


                //setup upcoming and recorded menu contents
                var uWebinars = _reposWebinars.GetUpcoming().OrderBy(w => w.Date).Take(15).ToList();


                var up = new StringBuilder();
                up.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Upcoming/'><b>View All Upcoming</b></a></li>");
                for (int i = 0; i < uWebinars.Count; i++)
                {
                    string shortTitle =
                        uWebinars[i].Title.Length > 35
                            ? uWebinars[i].Title.Substring(0, 35) + "..."
                            : uWebinars[i].Title;
                    up.Append("<li role=\"presentation\"><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                              uWebinars[i].idWebinar + "'>" + shortTitle + "</a></li>");
                }
                Session["upcoming"] = up;

                var rWebinars = _reposWebinars.GetRecorded().OrderBy(w => w.Date).Take(15).ToList();
                StringBuilder rec = new StringBuilder();
                rec.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/recorded/'><b>View All Recordings</b></a></li>");
                for (int i = 0; i < 8; i++)
                {
                    string ShortTitle =
                        rWebinars[i].Title.Length > 45
                            ? rWebinars[i].Title.Substring(0, 45) + "..."
                            : rWebinars[i].Title;

                    rec.Append("<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                               rWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>");
                }
                Session["rec"] = rec;

                StringBuilder allActiveEvents = new StringBuilder();
                allActiveEvents.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/'><b>View All Events</b></a></li>");
                for (int i = 0; i < uWebinars.Count; i++)
                {
                    string ShortTitle =
                        uWebinars[i].Title.Length > 45
                            ? uWebinars[i].Title.Substring(0, 45) + "..."
                            : uWebinars[i].Title;

                    allActiveEvents.Append(
                        "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                        uWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>");
                }

                if (uWebinars.Count < 9)
                {
                    var fillToTotalCount = 9 - uWebinars.Count;
                    for (int i = 0; i < fillToTotalCount; i++)
                    {
                        string ShortTitle =
                            rWebinars[i].Title.Length > 45
                                ? rWebinars[i].Title.Substring(0, 45) + "..."
                                : rWebinars[i].Title;

                        allActiveEvents.Append(
                            "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                            rWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>");
                    }
                }
                Session["allEvents"] = allActiveEvents;
            }
        }
    }
}