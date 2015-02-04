using System.Configuration;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Controllers;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Services;
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

    public class MvcApplication : HttpApplication
    {
        readonly IErrorResponseCommand errorResponseCommand = new ErrorResponseCommand();
        public static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IStateService StateService = new StateService();

        const string HtmlBreak = "<br />";
        const string SessionStartError = "ERROR: --SESSION START-- ";
        const char Pipe = '|';

        protected void Application_Start()
        {
            // Clears all previously registered view engines.
            ViewEngines.Engines.Clear();

            // Registers our Razor C# specific view engine.
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();

            //Database.SetInitializer<TTSWebinarsContext>(new DatabaseInitializer());
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            MapperConfig.Initialize();

            log4net.Config.XmlConfigurator.Configure();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
        }
        //https://www.simple-talk.com/dotnet/asp.net/handling-errors-effectively-in-asp.net-mvc/
        protected void Application_Error(object sender, EventArgs e)
        {
            //  Variables related to error and context.
            var httpContext = ((MvcApplication)sender).Context;
            Exception exception = Server.GetLastError();
            var httpException = exception as HttpException;

            //  Variables for the error controller and action.
            var controller = new StaticContentController();

            // this will hold data about the new route to the custom error page
            var newRouteData = new RouteData(); 

            var errorResponse = new ErrorResponse
            {
                Controller = controller,
                HttpContext = httpContext,
                ExceptionInstance = exception,
                NewrouteData = newRouteData
            };


            newRouteData.Values[WebUiConstants.Controller] = WebUiConstants.StaticContent;

            // This instructs IIS to ignore it's own default error pages, allowing us to use our own.
            // Note: in Web.config, httpErrors is as follows: <httpErrors existingResponse="PassThrough" />
            // This is required (or set existingResponse to "auto"), otherwise Response.TrySkipIisCustomErrors
            // is ignored.
            Response.TrySkipIisCustomErrors = true;

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        Response.StatusCode = 404;
                        newRouteData.Values[WebUiConstants.Action] = WebUiConstants.PageNotFound;
                        errorResponseCommand.Execute(errorResponse);
                        break;

                    case 500:
                        Response.StatusCode = 500;
                        newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                        errorResponseCommand.Execute(errorResponse);
                        break;

                    default:
                        Response.StatusCode = 500;
                        newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                        errorResponseCommand.Execute(errorResponse);
                        break;
                }
            }
            else
            {
                Response.StatusCode = 500;
                newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                errorResponseCommand.Execute(errorResponse);
            }

        }


        private void Session_Start(object sender, EventArgs e)
        {
            //avoids session state when the WebAPI method is invoked.
            if (System.Web.HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != "~/account/get/")
            {
                TTSWebinarsContext ttsWebinarsContext = new TTSWebinarsContext();
                try
                {
                    IAffiliateRepository affiliateRepository = new AffiliateRepository(ttsWebinarsContext);
                    IWebinarRepository _reposWebinars = new WebinarRepository(ttsWebinarsContext);
                    IInstitutionRepository institutionRepository = new InstitutionRepository(ttsWebinarsContext);

                    //StateService.SetValue("searchTerm", string.Empty);
                    //StateService.SetValue("IncludeRecorded", false);
                    //StateService.SetValue("IncludeUpcoming", true);
                    //StateService.SetValue("searchExtent", "Upcoming");

                    StateService.SetValue(WebUiConstants.CurrentAffiliate,
                        affiliateRepository.FindByIdWithIncluding(19, a => a.WebUser));
                    StateService.SetValue("AValidInstitution", institutionRepository.FindFirst());

                    //This session var lets us understand the origin of the Affiliate session - 
                    //...answers the question - How was the Session Affiliate determined?
                    //Here we the initial value to 'default' ... later code will
                    // override if conditions dictate.
                    StateService.SetValue("AffiliateSessionSource", "default" + Pipe + AppConst.DEFAULT_AFFILIATE);
                    StateService.SetValue(WebUiConstants.SubdomainBranding,
                        ConfigurationManager.AppSettings[AppConst.TESTING_URL]);

                    //following are values to be stored for audit purposes.
                    if (HttpContext.Current.Request.UrlReferrer != null)
                        StateService.SetValue(WebUiConstants.SubdomainBranding,
                            HttpContext.Current.Request.UrlReferrer.ToString().Trim());
                    StateService.SetValue("FirstPage", HttpContext.Current.Request.Url.ToString().Trim());
                    StateService.SetValue("InitialQueryString", Request.Url.Query);
                    StateService.SetValue(WebUiConstants.SessionId, HttpContext.Current.Session.SessionID);

                    //DETERMINE CURRENT AFFILIATE
                    //MEHTOD 1: VIA QUERY STRING -- idAff=[idUserAff]   
                    if (!string.IsNullOrEmpty(Request.QueryString[WebUiConstants.AffiliateId]))
                    {

                        int loadAff;

                        if (int.TryParse(Request.QueryString[WebUiConstants.AffiliateId], out loadAff))
                        {
                            try
                            {
                                StateService.SetValue("AffiliateSessionSource",
                                    WebUiConstants.AffiliateId + Pipe + loadAff);
                                // determine the current affiliate
                                StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                    affiliateRepository.FindByIdWithIncluding(loadAff, a => a.WebUser));
                            }
                            catch (Exception ex)
                            {
                                logger.Fatal(ex);
                                logger.Info(SessionStartError + "failed to load idAff code: " +
                                            HttpContext.Current.Request.Url.ToString());
                                throw;
                            }
                        }
                        else
                        {
                            logger.Error(SessionStartError + "Non-numeric idAff: " +
                                         HttpContext.Current.Request.QueryString.ToString());
                        }
                    }


                    var subdomainBranding = StateService.GetValue<string>(WebUiConstants.SubdomainBranding);
                    //METHOD 2: VIA THE DOMAIN NAME BEING RUN
                    //e.g. http://webinars.cftws.org - means CurrentAffiliate should be 'cftws'
                    if (!string.IsNullOrEmpty(subdomainBranding))
                    {
                        var subdomainBrandType = subdomainBranding.Split('.').FirstOrDefault();
                        if (!string.IsNullOrEmpty(subdomainBrandType) &&
                            subdomainBrandType.Equals(WebUiConstants.Webinars, StringComparison.OrdinalIgnoreCase))
                        {
                            //we discover we are running with an affiliate's subdomain
                            string affilliateDomain =
                                ConfigurationManager.AppSettings[AppConst.TESTING_URL].Split('.')[1];
                            try
                            {
                                StateService.SetValue("AffiliateSessionSource", "Sub" + Pipe + affilliateDomain);
                                //   the name of the property 'ttsDomain' is the abbreviated name chosen
                                //   for use (as a shortcut or nicname) by us to refer to a given affiliate. It may or may not
                                //   be literally the Domain Name used by the given affiliate.

                                StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                    affiliateRepository.LoadByTTSDomain(affilliateDomain) ??
                                    affiliateRepository.LoadByTTSDomain("bennett"));
                            }
                            catch (Exception ex)
                            {
                                logger.Fatal(ex);
                                throw;
                            }
                        }
                    }

                    /*METHOD 3: VIA THE SUBDOMAIN -- implementation of this method is on hold pending SEO considerations.
                //  when passed 'cftws.bankwebinars.com' this method
                 
                //  set the Session Affiliate to 380
                //    
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
                //    
                //}
                */
                    logger.Info("Start Session: " + StateService.GetValue<string>(WebUiConstants.SessionId));

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

                    StateService.SetValue("FirstCookies", allCookies.ToString());

                    //setup upcoming and recorded menu contents
                    //var upcomingWebinars = _reposWebinars.GetUpcoming().OrderBy(w => w.Date).Take(8).ToList();
                    //var upComingPresentationListItems = new StringBuilder();
                    //upComingPresentationListItems.Append(
                    //    "<li role='presentation'><a  role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/?eventsToShow=upcoming'>View <b>All</b> Upcoming Events</a></li>");

                    //if (upcomingWebinars.Count > 0)
                    //{
                    //    for (int i = 0; i < upcomingWebinars.Count; i++)
                    //    {
                    //        var upcomingWebinar = upcomingWebinars[i];
                    //        string shortTitle = upcomingWebinar.Title.Length > 35
                    //                ? upcomingWebinar.Title.Substring(0, 35) + "..."
                    //                : upcomingWebinar.Title;

                    //        string seoTitle = upcomingWebinar.Title.RemoveIllegalCharacters().ReplaceSpacesWithHyphens().ToLower().TrimEnd('.');

                    //        upComingPresentationListItems.Append(
                    //            string.Format("<li role=\"presentation\"><a role=\"menuitem\" tabindex=\"-1\" href='/{0}/{1}'", upcomingWebinar.idWebinar, seoTitle) +
                    //            upcomingWebinars[i].idWebinar + "'>" + Server.HtmlEncode(shortTitle) + "</a></li>"
                    //            );
                    //    }

                    //}
                    //StateService.SetValue("upcoming", upComingPresentationListItems);



                    //var recordedWebinars = _reposWebinars.GetRecorded().OrderBy( w => w.Date).Take(8).ToList();
                    //StringBuilder recordedWebinarsListItems = new StringBuilder();
                    //recordedWebinarsListItems.Append(
                    //    "<li role='presentation'><a  role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/?eventsToShow=recorded'>View <b>All</b> Recordings</a></li>");
                    //if (recordedWebinars.Count > 0)
                    //{
                    //    for (int i = 0; i < 8; i++)
                    //    {
                    //        string ShortTitle =
                    //            recordedWebinars[i].Title.Length > 45
                    //                ? recordedWebinars[i].Title.Substring(0, 45) + "..."
                    //                : recordedWebinars[i].Title;

                    //        recordedWebinarsListItems.Append(
                    //            "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                    //            recordedWebinars[i].idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>"
                    //            );
                    //    }

                    //}
                    //StateService.SetValue("rec", recordedWebinarsListItems);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    //ttsWebinarsContext.Database.Connection.Close(); --> PROBABLY NOT NECESSARY. DISPOSE SHOULD DO THIS FOR US.
                    ttsWebinarsContext.Dispose();
                }
            }
        }
    }
}