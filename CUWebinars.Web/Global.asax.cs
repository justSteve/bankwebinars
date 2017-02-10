using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Controllers;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Services;
using log4net;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;
using ClaimTypes = System.IdentityModel.Claims.ClaimTypes;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using Elmah;
using GemBox.Document;

namespace CUWebinars.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        readonly IErrorResponseCommand _errorResponseCommand = (IErrorResponseCommand)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IErrorResponseCommand));
        public static ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IStateService StateService = new StateService();

        public const string SessionStartError = "ERROR: --SESSION START-- ";
        const char Pipe = '|';

        protected void Application_Start()
        {
            // Clears all previously registered view engines.
            ViewEngines.Engines.Clear();

            ComponentInfo.SetLicense("DUZ7-YWDS-BDTM-7Z5I");
            // Registers our Razor C# specific view engine.
            ViewEngines.Engines.Add(new RazorViewEngine() { FileExtensions = new string[] { "cshtml" } });

            AreaRegistration.RegisterAllAreas();

            //Database.SetInitializer<TTSWebinarsContext>(new DatabaseInitializer());
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            MapperConfig.Initialize();

#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif

            ConfigureLogging();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Email;
            //GetSmtpDetails();
            LogStartupDetails();

            //InvokeGhostTests();
        }

        private static void ConfigureLogging()
        {
            const string infrastructureLogconfigs = @"Infrastructure/LogConfigs";

            switch (GlobalConfig.GlobalConfigSingleton.Tenant)
            //switch ("Dave")
            {
                case DomainConstants.BankWebinars:
                    if (!Debugger.IsAttached)
                    {
                        log4net.Config.XmlConfigurator.Configure(
                            new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                                "BWLog4net.xml")));
                        //"BWLocal.xml")));
                    }
                    else
                    {
                        log4net.Config.XmlConfigurator.Configure(
                           new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                               "BWLocal.xml")));
                    }
                    break;

                case DomainConstants.CUWebinars:
                    if (!Debugger.IsAttached)
                    {
                        log4net.Config.XmlConfigurator.Configure(
                            new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                                "CULog4net.xml")));

                    }
                    else
                    {
                        //log4net.Config.XmlConfigurator.Configure(
                        //    new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                        //        "CULog4net.xml")));

                        log4net.Config.XmlConfigurator.Configure(
                           new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                               "CULocal.xml")));
                    }

                    break;
                case "Dave":
                    log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs, "DARLog4net.xml")));
                    break;
                default:
                    throw new NotSupportedException(string.Format("There is no log4net configuration for {0}", GlobalConfig.GlobalConfigSingleton.Tenant));
            }
        }

        private static void LogStartupDetails()
        {
            const string keyValuePattern = "Key - {0}, Value - {1}{2}";
            var LoggerInfoString = "";
            var TenantName = "";

            foreach (string key in ConfigurationManager.AppSettings)
            {
                string value = ConfigurationManager.AppSettings[key];

                //LoggerInfoString += string.Format("HostApp: Key {0} Value {1}", key, value);
                LoggerInfoString += string.Format("HostApp {0} : {1}{2}", key, value, Environment.NewLine);

                if (key == "Tenant")
                {
                    TenantName = value;
                }
            }

            logger.Info(string.Format("{0} Spinning up application: ", TenantName));
            logger.Info(string.Format("Config Properties - {0}", GlobalConfig.GlobalConfigSingleton.PropertiesAsString));
            logger.Info(string.Format("AppInfo: {0}", GetAppVersionInfo()));

            var smtpDetails = GetSmtpDetails();

            LoggerInfoString += string.Format(keyValuePattern, "UserName", smtpDetails.UserName, Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "Password", smtpDetails.Password, Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "Host", smtpDetails.Host, Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "DefaultCredentials", smtpDetails.DefaultCredentials,
                Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "ClientDomain", smtpDetails.ClientDomain, Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "Ssl", smtpDetails.EnableSsl, Environment.NewLine);
            LoggerInfoString += string.Format(keyValuePattern, "Port", smtpDetails.Port, Environment.NewLine);

            logger.Info(string.Format("AppSettings for {0} are: {1}", TenantName, LoggerInfoString));
        }

        private static string GetAppVersionInfo()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(asm.Location);
            return fileVersionInfo.FileVersion;
        }
        private static SmtpNetworkElement GetSmtpDetails()
        {
            var smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            if (smtpSection == null)
                throw new NullReferenceException("There is no SMTP section in this App.config file.");

            var mailSettings = smtpSection.Network;

            return mailSettings;
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
            ; Response.TrySkipIisCustomErrors = true;

            if (httpException != null)
            {
                var userName = "anon";
                if (User.Identity.IsAuthenticated)
                {
                    userName = User.Identity.Name;
                }
                //ErrorSignal.FromCurrentContext().Raise(exception);
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        Response.StatusCode = 404;
                        newRouteData.Values[WebUiConstants.Action] = WebUiConstants.PageNotFound;
                        _errorResponseCommand.Execute(errorResponse);
                        break;

                    case 500:
                        if (User.Identity.IsAuthenticated
                            && User.Identity.Name.StartsWith("admin")
                            && User.Identity.Name.EndsWith("ttstrain.com"))
                        {
                            logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session);

                            //Now that we know we have an authenticated/authorized Admin
                            // no need to hide sensitive info. Figure out how to dump the Context's error message
                            // instead of the generic boiler plate.
                            newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                            _errorResponseCommand.Execute(errorResponse);
                        }
                        else
                        {
                            logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session);

                            newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                            _errorResponseCommand.Execute(errorResponse);
                        }
                        break;
                    default:
                        Response.StatusCode = 500;
                        logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session);

                        newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                        _errorResponseCommand.Execute(errorResponse);
                        break;
                }
            }
            else
            {
                Response.StatusCode = 500;
                newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                _errorResponseCommand.Execute(errorResponse);
            }

        }


        private void Session_Start(object sender, EventArgs e)
        {
            var ua = Request.UserAgent;
            if (ua == null)
                ua = "bot";
            bool iscrawler = Regex.IsMatch(ua,
                @"bot|crawler|baiduspider|80legs^|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google",
                RegexOptions.IgnoreCase);
            if (!iscrawler)
            {
                var ttsWebinarsContext = new TTSWebinarsContext();
                try
                {
                    IAffiliateRepository affiliateRepository = new AffiliateRepository(ttsWebinarsContext);
                    IWebUserRepository webUserRepository = new WebUserRepository(ttsWebinarsContext);
                    IInstitutionRepository institutionRepository = new InstitutionRepository(ttsWebinarsContext);


                    StateService.SetValue(WebUiConstants.CurrentAffiliate,
                        affiliateRepository.FindByIdWithIncluding(AppConst.DEFAULT_AFFILIATE));
                    //StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.FindByIdWithIncluding(AppConst.DEFAULT_AFFILIATE, a => a.WebUser));
                    StateService.SetValue("AValidInstitution", institutionRepository.FindFirst());

                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity) User.Identity;

                        if (claimsIdentityOfAuthenticatedUser.HasClaim(
                            (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate))
                        {
                            var claimTTSDomain =
                                claimsIdentityOfAuthenticatedUser.Claims.Where(
                                        c => c.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate)
                                    .First()
                                    .Value;
                            StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                affiliateRepository.LoadByTTSDomain(claimTTSDomain));
                        }
                        else
                        {
                            StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                webUserRepository.FindAffiliateOfLastOrder(User.Identity.Name));
                        }
                        if (claimsIdentityOfAuthenticatedUser.HasClaim(
                            (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Admin))
                        {

                            StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                affiliateRepository.LoadById(19));
                        }
                    }

                    //This session var lets us understand the origin of the Affiliate session - 
                    //...answers the question - How was the Session Affiliate determined?
                    //Here we the initial value to 'default' ... later code will
                    // override if conditions dictate.
                    StateService.SetValue("AffiliateSessionSource", "default" + Pipe + AppConst.DEFAULT_AFFILIATE);
                    //StateService.SetValue(WebUiConstants.SubdomainBranding, ConfigurationManager.AppSettings[AppConst.TESTING_URL]);  //TODO: [dar] I think this can be deleted

                    //following are values to be stored for audit purposes.
                    if (HttpContext.Current != null && HttpContext.Current.Request.UrlReferrer != null)
                        StateService.SetValue(WebUiConstants.SubdomainBranding,
                            HttpContext.Current.Request.UrlReferrer.ToString().Trim());

                    if (HttpContext.Current != null)
                    {
                        StateService.SetValue(WebUiConstants.FirstPage,
                            HttpContext.Current.Request.Url.ToString().Trim());
                        StateService.SetValue(WebUiConstants.InitialQueryString, Request.Url.Query);
                        StateService.SetValue(WebUiConstants.SessionId, HttpContext.Current.Session.SessionID);

                        //DETERMINE CURRENT AFFILIATE
                        //MEHTOD 1: VIA QUERY STRING -- idAff=[idUserAff]   
                        if (!string.IsNullOrEmpty(Request.QueryString[WebUiConstants.AffiliateId]))
                        {
                            int loadAff;

                            if (int.TryParse(Request.QueryString[WebUiConstants.AffiliateId], out loadAff))
                            {
                                //Affiliate foundAff = affiliateRepository.FindByIdWithIncluding(loadAff, a => a.WebUser);
                                Affiliate foundAff = affiliateRepository.FindByIdWithIncluding(loadAff);

                                if (!ReferenceEquals(foundAff, null))
                                {
                                    StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                        affiliateRepository.FindByIdWithIncluding(loadAff));
                                    //StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.FindByIdWithIncluding(loadAff, a => a.WebUser));
                                    logger.Info(string.Format("Resolving Affiliate via query string with id {0}",
                                        loadAff));

                                }
                                else
                                {
                                    logger.Info(SessionStartError + "failed to load idAff code: " +
                                                HttpContext.Current.Request.Url);
                                }
                                //    tries to lighten the expense of session start by removing try/catch
                                //try
                                //{
                                //    StateService.SetValue("AffiliateSessionSource", WebUiConstants.AffiliateId + Pipe + loadAff);
                                //    // determine the current affiliate
                                //    StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.FindByIdWithIncluding(loadAff, a => a.WebUser));
                                //    logger.Info(string.Format("Resolving Affiliate via query string with id {0}", loadAff));

                                //}
                                //catch (Exception ex)
                                //{
                                //    logger.Fatal(ex);
                                //    logger.Info(SessionStartError + "failed to load idAff code: " +
                                //                HttpContext.Current.Request.Url.ToString());
                                //    throw;
                                //}
                            }
                            else
                            {
                                logger.Error(SessionStartError + "Non-numeric idAff: " +
                                             HttpContext.Current.Request.QueryString);
                            }
                        }
                    }


                    var subdomainBranding = StateService.GetValue<string>(WebUiConstants.SubdomainBranding);
                    //METHOD 2: VIA THE DOMAIN NAME BEING RUN
                    //e.g. http://webinars.cftws.org - means CurrentAffiliate should be 'cftws'
                    if (!string.IsNullOrEmpty(subdomainBranding))
                    {
                        logger.Info(string.Format("Resolving Affiliate via subdomainBranding {0}", subdomainBranding));

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
                                logger.Error(string.Format("Failed to resolving Affiliate via subdomainBranding {0}",
                                    subdomainBranding));
                                logger.Fatal(ex);
                                throw;
                            }
                        }
                    }

                    var allCookies = new StringBuilder();

                    for (var i = 0; i < Request.Cookies.Count; i++)
                    {
                        HttpCookie aCookie = Request.Cookies[i];

                        if (i > 0) allCookies.Append(", ");
                        if (aCookie != null)
                        {

                            allCookies.Append("\"CookieName\": \"" + aCookie.Name);

                            if (aCookie.HasKeys)
                            {
                                NameValueCollection cookieValues = aCookie.Values;

                                string[] cookieValueNames = cookieValues.AllKeys;
                                allCookies.Append("\":  {\"SubKeys:\"");
                                for (int j = 0; j < cookieValues.Count; j++)
                                {
                                    string subkeyName = Server.HtmlEncode(cookieValueNames[j]);
                                    string subkeyValue = Server.HtmlEncode(cookieValues[j]);
                                    if (!subkeyName.StartsWith("__") &&
                                        !subkeyName.StartsWith("Fed")
                                    )
                                    {
                                        allCookies.Append("\"SubkeyName: \"" + subkeyName);
                                        allCookies.Append("\", \"SubkeyValue: \"" + subkeyValue);
                                    }
                                }
                                allCookies.Append("\"},");

                            }
                            else
                            {
                                allCookies.Append("\", \"Value\": \"" + Server.HtmlEncode(aCookie.Value) + "\"");
                            }
                        }
                    }

                    StateService.SetValue(WebUiConstants.FirstCookies, allCookies.ToString());
                    if (User.Identity.IsAuthenticated)
                    {
                        logger.Info("{\"Name\": \"" + User.Identity.Name
                                    + "\", \"FirstPage\": \"" + StateService.GetValue<string>(WebUiConstants.FirstPage)
                                    + "\", \"QueryString\": \"" +
                                    StateService.GetValue<string>(WebUiConstants.InitialQueryString)
                                    + "\", \"SessionId\": \"" + StateService.GetValue<string>(WebUiConstants.SessionId)
                                    + "\", \"FirstCookies\": {" +
                                    StateService.GetValue<string>(WebUiConstants.FirstCookies) + "}}"
                        );
                    }
                    else
                    {
                        logger.Info("Anon Session Starts with: {"
                                    + "\"FirstPage\": \"" + StateService.GetValue<string>(WebUiConstants.FirstPage)
                                    + "\", \"QueryString\": \"" +
                                    StateService.GetValue<string>(WebUiConstants.InitialQueryString)
                                    + "\", \"SessionId\": \"" + StateService.GetValue<string>(WebUiConstants.SessionId)
                                    + "\", \"FirstCookies\": {" +
                                    StateService.GetValue<string>(WebUiConstants.FirstCookies) + "}}"
                        );
                    }
                }
                catch (Exception exception)
                {
                    logger.Fatal("SessionStart Exception!!!", exception);
                    Console.WriteLine(exception);
                }
                finally
                {
                    //ttsWebinarsContext.Database.Connection.Close(); --> THIS LINE PROBABLY NOT NECESSARY. DISPOSE SHOULD DO THIS FOR US.
                    ttsWebinarsContext.Dispose();
                }
            } //ends attempt to filter bots
            else
            {

                var ttsWebinarsContext = new TTSWebinarsContext();
                IAffiliateRepository affiliateRepository = new AffiliateRepository(ttsWebinarsContext);

                StateService.SetValue(WebUiConstants.CurrentAffiliate,
                    affiliateRepository.FindByIdWithIncluding(19));
            }
        }

        //private void SessionAuthenticationModule_SessionSecurityTokenReceived(object sender, SessionSecurityTokenReceivedEventArgs e)
        //{
        //    // This method gets run about 5 or 6 times per page, seems to be run once for each resource request that the page includes
        //    // I am using "if (request.Path...)" tests to isolate the functionality to "page loads" for particular pages

        //    System.Diagnostics.Debug.WriteLine(Request.Path);

        //    if (Request.Path == "/Account/MyWebinars") // Proof-of-concept, watch for the /Admin (or pick a different page if you want) page load
        //    {
        //        // BREAKPOINT IN HERE and browse to /Admin (My Webinars as admin)

        //        var token = e.SessionToken;

        //        System.Diagnostics.Debug.WriteLine("in /Admin--");

        //        System.Diagnostics.Debug.WriteLine(token.ValidFrom.ToString("MM/dd/yyyy HH:mm:ss"));
        //        System.Diagnostics.Debug.WriteLine(token.ValidTo.ToString("MM/dd/yyyy HH:mm:ss"));

        //        //*** THE FOLLOWING DEFINITELY SEEMS TO SET THE COOKIE (updates the ValidFrom and ValidTo properties on subsequent access)
        //        // the question are:
        //        //  does it pull the latest claims from the DB?
        //        //  how fast (or slow) is this process?

        //        e.ReissueCookie = true;
        //        e.SessionToken =
        //            new SessionSecurityToken(
        //                token.ClaimsPrincipal,
        //                token.Context,
        //                DateTime.UtcNow,
        //                DateTime.UtcNow.Add(TimeSpan.FromHours(8))) // needs to be the configured value, hard-coded to 8 hours for testing
        //            {
        //                IsPersistent = token.IsPersistent,
        //                IsReferenceMode = token.IsReferenceMode
        //            };

        //        // these dates are not changed at this point in time, browse to the home page (or wherever you set up below as "subsequent")
        //        System.Diagnostics.Debug.WriteLine(" ---- ");
        //        System.Diagnostics.Debug.WriteLine(token.ValidFrom.ToString("MM/dd/yyyy HH:mm:ss"));
        //        System.Diagnostics.Debug.WriteLine(token.ValidTo.ToString("MM/dd/yyyy HH:mm:ss"));

        //        System.Diagnostics.Debug.WriteLine("end /Admin--");

        //    }

        //    if (Request.Path == "/Home") // could be any page that you want, this is the "subsequent" "page request" load where we can see the dates have been changed
        //    {
        //        // BREAKPOINT IN HERE and browse to homepage (/) after browsing to /Admin (or whatever is configured above)

        //        var token = e.SessionToken;
        //        System.Diagnostics.Debug.WriteLine("in /Homepage--");

        //        // these dates are different (updated) after the code above (/Admin) runs
        //        System.Diagnostics.Debug.WriteLine(token.ValidFrom.ToString("MM/dd/yyyy HH:mm:ss"));
        //        System.Diagnostics.Debug.WriteLine(token.ValidTo.ToString("MM/dd/yyyy HH:mm:ss"));
        //    }
        //}
    }
}