using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Core;
using CUWebinars.Web.Data.Repositories;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Services;
using CUWebinars.Web.Utils;
using log4net;
using System;
using System.Collections.Specialized;
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
        private static IStateService stateService = new StateService();

        const string AffiliateId = "idAff";
        const string HtmlBreak = "<br />";
        const string SessionStartError = "ERROR: --SESSION START-- ";
        const char Pipe = '|';

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
            GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;

            var bla = globalConfig.AppTenant;
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
                stateService.SetValue("searchTerm", string.Empty);
                stateService.SetValue("IncludeRecorded", false);
                stateService.SetValue("IncludeUpcoming", true);
                stateService.SetValue("searchExtent", "Upcoming");

                // determine the current affiliate
                stateService.SetValue("CurrentAffiliate", _repos.GetCurrentAffiliate());

                //This session var lets us understand the origin of the Affiliate session - 
                //...answers the question - How was the Session Affiliate determined?
                //Here we the initial value to 'default' ... later code will
                // override if conditions dictate.
                stateService.SetValue("AffiliateSessionSource", "default" + Pipe + AppConst.DEFAULT_AFFILIATE);
                stateService.SetValue("SubdomainBranding", @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL]);

                //following are values to be stored for audit purposes.
                if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
                    stateService.SetValue("SubdomainBranding", HttpContext.Current.Request.UrlReferrer.ToString().Trim());
                stateService.SetValue("FirstPage",HttpContext.Current.Request.Url.ToString().Trim());
                stateService.SetValue("InitialQueryString", Request.QueryString);
                stateService.SetValue("SessionID", HttpContext.Current.Session.SessionID);
                
                


                if (!String.IsNullOrEmpty(Request.QueryString[AffiliateId]))
                {
                    //DETERMINE CURRENT AFFILIATE
                    //MEHTOD 1: VIA QUERY STRING -- idAff=[idUserAff]                    
                    int loadAff;

                    if (int.TryParse(Request.QueryString[AffiliateId], out loadAff))
                    {
                        try
                        {
                            stateService.SetValue("AffiliateSessionSource", AffiliateId + Pipe + loadAff);
                            //TODO ... need updated method of loading affiliate by id
                            //CurrentSession.Instance.CurrentAffiliate = AffiliateFacade.Instance.Load(Convert.ToInt32(loadAff));
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex);
                            logger.Info(SessionStartError + "failed to load idAff code: " + HttpContext.Current.Request.Url.ToString());
                            throw;
                        }
                    }
                    else
                    {
                        logger.Error(SessionStartError + "Non-numeric idAff: " + HttpContext.Current.Request.QueryString.ToString());
                    }
                }


                var subdomainBranding = stateService.GetValue<string>("SubdomainBranding");

                if (!string.IsNullOrEmpty(subdomainBranding))
                {
                    var subdomainBrandType = subdomainBranding.Split('.').FirstOrDefault();

                    if (!string.IsNullOrEmpty(subdomainBrandType) && subdomainBrandType.Equals("webinars", StringComparison.OrdinalIgnoreCase))
                    {

                        //METHOD 2: VIA THE DOMAIN NAME BEING RUN
                        //e.g. http://webinars.cftws.org - means CurrentAffiliate should be 'cftws'

                        string affilliateDomain = @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL].Split('.')[1];

                        try
                        {
                            stateService.SetValue("AffiliateSessionSource", "Sub" + Pipe + affilliateDomain);
                            //todo: devise method to load Affiliate based on match to Affiliate.ttsDomain
                            //   the name of the property 'ttsDomain' is the abbreviated name chosen
                            //   for use (as a shortcut or nicname) by us to refer to a given affiliate. It may or may not
                            //   be literally the Domain Name used by the given affiliate.
                            //CurrentSession.Instance.CurrentAffiliate = AffiliateFacade.Instance.LoadByTTSDomain(affilliateDomain);
                            //HttpContext.Current.Session["CurrentAffiliate"] = AffiliateFacade.Instance.LoadByTTSDomain(affilliateDomain);

                            //if (AffiliateFacade.Instance.LoadByTTSDomain(affilliateDomain) == null)
                            //{
                            //    CurrentSession.Instance.CurrentAffiliate = AffiliateFacade.Instance.LoadByTTSDomain("bennett");
                            //    //HttpContext.Current.Session["CurrentAffiliate"] = AffiliateFacade.Instance.LoadByTTSDomain("bennett");
                            //}
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex);
                            throw;
                        }
                    }
                }

                //METHOD 3: VIA THE SUBDOMAIN
                //  when passed 'cftws.bankwebinars.com' this method
                //  set the Session Affiliate to 380
                //    public static int GetAffilliateByTTSDomain(string currentHost)
                //    {
                //        if (currentHost.ToLower() == "www" || currentHost.ToLower() == "testing" || currentHost.ToLower() == "bankwebinars" || currentHost == "localhost")
                //        {
                //            return 0;
                //        }
                //        try
                //        {
                //            return AffiliateFacade.Instance.LoadByTTSDomain(currentHost).ID;
                //        }
                //        catch (Exception ex)
                //        {
                //            Logger.Instance.LogException(ex);
                //            Logger.Instance.LogMessage("ERROR: GetAffilliateByTTSDomain: was passed: " + currentHost);
                //            return 0;
                //        }
                //    }
                //}
                
                logger.Info("Start Session: " + stateService.GetValue<string>("SessionID"));

                var allCookies = new StringBuilder();

                for (var i = 0; i < Request.Cookies.Count; i++)
                {
                    HttpCookie aCookie = Request.Cookies[i];

                    if (aCookie != null)
                    {
                        allCookies.Append("Name = " + aCookie.Name + HtmlBreak);

                        if (aCookie.HasKeys)
                        {
                            NameValueCollection cookieValues = aCookie.Values;

                            string[] cookieValueNames = cookieValues.AllKeys;

                            for (int j = 0; j < cookieValues.Count; j++)
                            {
                                string subkeyName = Server.HtmlEncode(cookieValueNames[j]);
                                string subkeyValue = Server.HtmlEncode(cookieValues[j]);
                                allCookies.Append("Subkey name = " + subkeyName + HtmlBreak);
                                allCookies.Append("Subkey value = " + subkeyValue + HtmlBreak + HtmlBreak);
                            }
                        }
                        else
                        {
                            allCookies.Append("Value = " + Server.HtmlEncode(aCookie.Value) + HtmlBreak + HtmlBreak);
                        }
                    }
                }

                stateService.SetValue("FirstCookies", allCookies.ToString());

                //setup upcoming and recorded menu contents
                var upcomingWebinars = _reposWebinars.GetUpcoming().OrderBy(w => w.Date).Take(15).ToList();

                var upComingPresentationListItems = new StringBuilder();

                upComingPresentationListItems.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Upcoming/'><b>View All Upcoming</b></a></li>"
                    );

                if (upcomingWebinars.Count > 0)
                {
                    for (int i = 0; i < upcomingWebinars.Count; i++)
                    {
                        string shortTitle =
                            upcomingWebinars[i].Title.Length > 35
                                ? upcomingWebinars[i].Title.Substring(0, 35) + "..."
                                : upcomingWebinars[i].Title;

                        upComingPresentationListItems.Append(
                            "<li role=\"presentation\"><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                            upcomingWebinars[i].idWebinar + "'>" + shortTitle + "</a></li>"
                            );
                    }
                }

                stateService.SetValue("upcoming", upComingPresentationListItems);

                var recordedWebinars = _reposWebinars.GetRecorded().OrderBy(w => w.Date).Take(15).ToList();

                StringBuilder recordedWebinarsListItems = new StringBuilder();

                recordedWebinarsListItems.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/recorded/'><b>View All Recordings</b></a></li>"
                    );

                if (recordedWebinars.Count > 0)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        string ShortTitle =
                            recordedWebinars[i].Title.Length > 45
                                ? recordedWebinars[i].Title.Substring(0, 45) + "..."
                                : recordedWebinars[i].Title;

                        recordedWebinarsListItems.Append(
                            "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                            recordedWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>"
                            );
                    }
                }

                stateService.SetValue("rec", recordedWebinarsListItems);

                StringBuilder allActiveEventsListItems = new StringBuilder();

                allActiveEventsListItems.Append(
                    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/'><b>View All Events</b></a></li>"
                    );

                for (int i = 0; i < upcomingWebinars.Count; i++)
                {
                    string ShortTitle =
                        upcomingWebinars[i].Title.Length > 45
                            ? upcomingWebinars[i].Title.Substring(0, 45) + "..."
                            : upcomingWebinars[i].Title;

                    allActiveEventsListItems.Append(
                        "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                        upcomingWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>"
                        );
                }

                if (upcomingWebinars.Count < 9)
                {
                    var fillToTotalCount = 9 - upcomingWebinars.Count;
                    for (int i = 0; i < fillToTotalCount; i++)
                    {
                        string ShortTitle =
                            recordedWebinars[i].Title.Length > 45
                                ? recordedWebinars[i].Title.Substring(0, 45) + "..."
                                : recordedWebinars[i].Title;

                        allActiveEventsListItems.Append(
                            "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                            recordedWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>"
                            );
                    }
                }
                
                stateService.SetValue("allEvents", allActiveEventsListItems);
            }
        }
    }
}