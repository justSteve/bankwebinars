using Newtonsoft.Json;
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
using Microsoft.ApplicationInsights;
using RazorEngine.Compilation.ImpromptuInterface;

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
        public class GetCurrentIp
        {
            public override string ToString()
            {
                if (null != HttpContext.Current && HttpContext.Current.Session != null)
                {
                    return HttpContext.Current.Session["Ip"].ToString();
                }
                return string.Empty; // or "[No Page]" if you prefer
            }
        }
        protected void Application_Start()
        {
            // Clears all previously registered view engines.
            ViewEngines.Engines.Clear();
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

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
                case DomainConstants.DirectorSeries:
                    if (!Debugger.IsAttached)
                    {
                        log4net.Config.XmlConfigurator.Configure(
                            new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                                "DESLog4net.xml")));
                        //"BWLocal.xml")));
                    }
                    else
                    {
                        log4net.Config.XmlConfigurator.Configure(
                           new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                               "DESLocal.xml")));
                    }
                    break;

                case DomainConstants.CCS:
                    if (!Debugger.IsAttached)
                    {
                        log4net.Config.XmlConfigurator.Configure(
                            new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                                "CCSLog4net.xml")));
                        //"BWLocal.xml")));
                    }
                    else
                    {
                        log4net.Config.XmlConfigurator.Configure(
                           new FileInfo(Path.Combine(HttpRuntime.AppDomainAppPath, infrastructureLogconfigs,
                               "CCSLocal.xml")));
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
            var appErrMsg = new StringBuilder();
            var sessionId = "non Ini";
            try
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
                    sessionId = "no session found";
                    var userIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]; 

                    if (httpContext.Session != null && httpContext.Session.SessionID != null)
                        sessionId = httpContext.Session.SessionID;
                    
                    if (User.Identity.IsAuthenticated)
                    {
                        userName = User.Identity.Name + "_" + userIp;
                    }
                    appErrMsg.Append(" AppError Starts for " + userName + " at " + sessionId + " with error " + httpException.GetHttpCode());
                    
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
                                logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session.SessionID + " context: "
                                             + httpContext.Request.Path);

                                //Now that we know we have an authenticated/authorized Admin
                                // no need to hide sensitive info. Figure out how to dump the Context's error message
                                // instead of the generic boiler plate.
                                newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                                _errorResponseCommand.Execute(errorResponse);
                            }
                            else
                            {
                                logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session.SessionID + " context: "
                                    + httpContext.Request.Path);

                                newRouteData.Values[WebUiConstants.Action] = WebUiConstants.ServerErrorPage;
                                _errorResponseCommand.Execute(errorResponse);
                            }
                            break;
                        default:
                            Response.StatusCode = 500;
                            logger.Fatal("Global asax: " + userName + " error: " + exception.Message + " session: " + httpContext.Session.SessionID + " context: "
                                         + httpContext.Request.Path);
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
            catch (Exception e1)
            {
                appErrMsg.Append(" ERROR!! " + e1.Message + " at " + sessionId);
                Console.WriteLine(e1);

                telemetry.TrackException(e1);
            }

            telemetry.TrackException(Server.GetLastError());
        }

        protected void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            log4net.ThreadContext.Properties["Ip"] = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }
        private TelemetryClient telemetry = new TelemetryClient();
        private void Session_Start(object sender, EventArgs e)
        {
            
            var ua = Request.UserAgent;
            if (ua == null)
                ua = "bot";
            bool iscrawler = Regex.IsMatch(ua,
                @"search|spider|crawl|Bot|Monitor|BrowserMob|BingPreview|PagePeeker|WebThumb|URL2PNG|ZooShot|GomezA|Google SketchUp|Read Later|KTXN|KHTE|Keynote|Pingdom|AlwaysOn|zao|borg|oegp|silk|Xenu|zeal|NING|htdig|lycos|slurp|teoma|voila|yahoo|Sogou|CiBra|Nutch|Java|JNLP|Daumoa|Genieo|ichiro|larbin|pompos|Scrapy|snappy|speedy|vortex|favicon|indexer|Riddler|scooter|scraper|scrubby|WhatWeb|WinHTTP|voyager|archiver|Icarus6j|mogimogi|Netvibes|altavista|charlotte|findlinks|Retreiver|TLSProber|WordPress|wsr-agent|http client|Python-urllib|AppEngine-Google|semanticdiscovery|facebookexternalhit|web/snippet|Google-HTTP-Java-Client|bot|crawler|baiduspider|80legs^|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google",
                RegexOptions.IgnoreCase);
            if (!iscrawler)
            {
                var userIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (userIp == null)
                    userIp = "na";
                StateService.SetValue(WebUiConstants.Ip, userIp);
                var appInsightUser = new MyTelemetryInitializer();

                //GlobalContext.Properties["Ip"] = new GetCurrentIp();
                var userName = "anon";
                if (User.Identity.IsAuthenticated)
                {
                    userName = User.Identity.Name + "_" + userIp;
                }
                var ttsWebinarsContext = new TTSWebinarsContext();
                try
                {
                    IAffiliateRepository affiliateRepository = new AffiliateRepository(ttsWebinarsContext);
                    IWebUserRepository webUserRepository = new WebUserRepository(ttsWebinarsContext);
                    IInstitutionRepository institutionRepository = new InstitutionRepository(ttsWebinarsContext);


                    StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.FindByIdWithIncluding(AppConst.DEFAULT_AFFILIATE));
                    StateService.SetValue("AValidInstitution", institutionRepository.FindFirst());
                    //sessionStarter  = new StringBuilder();
                    var ss = new StringBuilder();
                    string sessionId = "";
                    //This session var lets us understand the origin of the Affiliate session - 
                    //...answers the question - How was the Session Affiliate determined?
                    //Here we the initial value to 'default' ... later code will
                    // override if conditions dictate.
                    StateService.SetValue("AffiliateSessionSource", "default" + Pipe + AppConst.DEFAULT_AFFILIATE);
                    if (HttpContext.Current != null)
                    {
                        sessionId = HttpContext.Current.Session.SessionID;

                        StateService.SetValue(WebUiConstants.FirstPage, HttpContext.Current.Request.Url.ToString().Trim());
                        StateService.SetValue(WebUiConstants.InitialQueryString, Request.Url.Query);
                        StateService.SetValue(WebUiConstants.SessionId, sessionId);
                        ss.Append("Starting for " + userName + " at " + sessionId);
                    }
                    else
                    {
                        ss.Append("HttpCtxIsNull!");
                        StateService.SetValue(WebUiConstants.FirstPage, "null");
                        StateService.SetValue(WebUiConstants.InitialQueryString, "null");
                        StateService.SetValue(WebUiConstants.SessionId, "null");
                        logger.Warn("SessionStart: HttpContext.Current is null!!!");
                    }

                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                        //
                        if (claimsIdentityOfAuthenticatedUser.HasClaim(
                            (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Affiliate))
                        {
                            ss.Append($" Is found to be Admin {sessionId}");

                            var claimTTSDomain = claimsIdentityOfAuthenticatedUser.Claims.Where(c => c.Type ==
                                CUWebinars.Business.Constants.ClaimTypes.Affiliate).First().Value;
                            ss.Append($" Is affiliate from {claimTTSDomain}");
                        }
                        else
                        {
                            if (claimsIdentityOfAuthenticatedUser.HasClaim(
                                (claim) => claim.Type == CUWebinars.Business.Constants.ClaimTypes.Admin))
                            {
                                ss.Append($" Is admin {User.Identity.Name}");

                                StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.LoadById(19));
                            }
                            else
                            {
                                try
                                {
                                    var findAff = webUserRepository.FindAffiliateForSession(User.Identity.Name);

                                    // authenticated end-user
                                    if (findAff != null)
                                    {
                                        StateService.SetValue(WebUiConstants.CurrentAffiliate, findAff);
                                        StateService.SetValue("AffiliateSessionSource", "FindAffiliateForSession" + Pipe + findAff);
                                        ss.Append(
                                            $" Is enduser named {userName} at {sessionId} associated with {findAff}");
                                        logger.Info(string.Format("SessionStart: Resolved Affiliate via FindAffiliateForSession {0} {1}", findAff.idUserAff, StateService.GetValue<string>(WebUiConstants.SessionId)));
                                    }
                                    else
                                    {
                                        StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.LoadById(19));
                                        StateService.SetValue("AffiliateSessionSource", "SessionAffIsNull!!" + Pipe + 19);

                                        ss.Append(
                                            $" Is misaffiliated enduser from {userName} at {sessionId} associated with {findAff}");
                                        logger.Warn(string.Format("SessionStart: unable to resolve Affiliate via FindAffiliateForSession {0} {1}", User.Identity.Name, StateService.GetValue<string>(WebUiConstants.SessionId)));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    StateService.SetValue(WebUiConstants.CurrentAffiliate, affiliateRepository.LoadById(19));
                                    StateService.SetValue("AffiliateSessionSource", "SessionHitEx!!" + ex.Message + " at " + sessionId + Pipe + 19);
                                    ss.Append($" ERROR! Hit ex {ex.Message} at {sessionId}");

                                    logger.Fatal("SessionStart: FindAffiliateForSession " + StateService.GetValue<string>(WebUiConstants.SessionId), ex);
                                }
                            }
                        }
                    }


                    //DETERMINE CURRENT AFFILIATE
                    //VIA QUERY STRING -- idAff=[idUserAff]   
                    if (!string.IsNullOrEmpty(Request.QueryString[WebUiConstants.AffiliateId]))
                    {
                        int loadAff;

                        if (int.TryParse(Request.QueryString[WebUiConstants.AffiliateId], out loadAff))
                        {
                            Affiliate foundAff = affiliateRepository.FindByIdWithIncluding(loadAff);
                            
                            if (!ReferenceEquals(foundAff, null))
                            {
                                ss.Append($" Found the affiliate to be {foundAff.ttsDomain} at {sessionId}");

                                StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                    affiliateRepository.FindByIdWithIncluding(loadAff));
                                StateService.SetValue("AffiliateSessionSource",
                                    "QueryString" + Pipe + affiliateRepository.FindByIdWithIncluding(loadAff));

                                logger.Info(string.Format(
                                    "SessionStart: " + StateService.GetValue<string>(WebUiConstants.SessionId) +
                                    " Resolving Affiliate via query string with id   {0}", loadAff));
                            }
                            else
                            {
                                StateService.SetValue(WebUiConstants.CurrentAffiliate,
                                    affiliateRepository.FindByIdWithIncluding(19));
                                StateService.SetValue("AffiliateSessionSource",
                                    "QueryString" + Pipe + affiliateRepository.FindByIdWithIncluding(19));

                                ss.Append($" Did not find the affiliate {sessionId}");

                                logger.Info("SessionStart: failed to load idAff code: " + sessionId);
                            }
                        }
                        else
                        {
                            if (HttpContext.Current != null)
                            {
                                logger.Error("SessionStart: Non-numeric idAff: " +
                                             HttpContext.Current.Request.QueryString + "_" + sessionId);
                                ss.Append($" Did not find the affiliate in qstring "+ HttpContext.Current.Request.QueryString + "_" + sessionId);
                            }
                            else
                            {
                                logger.Error("SessionStart: HttpContext was null??!!: " + "_" + sessionId);
                                ss.Append($" HttpContext was null??!! {sessionId}");
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
                            allCookies.Append("\"CookieName_" + sessionId + "\": \"" + aCookie.Name);

                            if (aCookie.HasKeys)
                            {
                                NameValueCollection cookieValues = aCookie.Values;

                                string[] cookieValueNames = cookieValues.AllKeys;
                                allCookies.Append("\":  {\"SubKeys:\"");
                                for (int j = 0; j < cookieValues.Count; j++)
                                {
                                    string subkeyName = Server.HtmlEncode(cookieValueNames[j]);
                                    string subkeyValue = Server.HtmlEncode(cookieValues[j]);
                                    ss.Append($"Found cookie {subkeyName} with {subkeyValue}");

                                    if (!subkeyName.StartsWith("__") &&
                                        !subkeyName.StartsWith("Fed")
                                    )
                                    {
                                        ss.Append($"and {subkeyName} with {subkeyValue}");

                                        allCookies.Append("\"SubkeyName: \"" + subkeyName);
                                        allCookies.Append("\", \"SubkeyValue: \"" + subkeyValue);
                                    }
                                }
                                allCookies.Append("\"},");

                            }
                            else
                            {

                                allCookies.Append("\", \"Value\": \"" + Server.HtmlEncode(aCookie.Value) + "\"");
                                ss.Append($"Cookie had no keys at {sessionId}");
                            }
                        }
                    }

                    StateService.SetValue(WebUiConstants.FirstCookies, allCookies.ToString());
                    if (User == null)
                        ss.Append(" User is found to be null!! " + sessionId);
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        logger.Info("Authenticated Session Starts with: {\"Name\": \"" + User.Identity.Name
                                    + "\", \"FirstPage\": \"" + StateService.GetValue<string>(WebUiConstants.FirstPage)
                                    + "\", \"QueryString\": \"" + StateService.GetValue<string>(WebUiConstants.InitialQueryString)
                                    + "\", \"SessionId\": \"" + StateService.GetValue<string>(WebUiConstants.SessionId)
                                    + "\", \"FirstCookies\": {" + StateService.GetValue<string>(WebUiConstants.FirstCookies) + "}}"
                        );
                        ss.Append(" Authenticated Session Starts with: {\"Name\": \"" + User.Identity.Name
                                    + "\", \"FirstPage\": \"" + StateService.GetValue<string>(WebUiConstants.FirstPage)
                                    + "\", \"QueryString\": \"" + StateService.GetValue<string>(WebUiConstants.InitialQueryString)
                                    + "\", \"SessionId\": \"" + StateService.GetValue<string>(WebUiConstants.SessionId)
                                    + "\", \"FirstCookies\": {" + StateService.GetValue<string>(WebUiConstants.FirstCookies) + "}}"
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
                        ss.Append(" Anon Session Starts with: {"
                                    + "\"FirstPage\": \"" + StateService.GetValue<string>(WebUiConstants.FirstPage)
                                    + "\", \"QueryString\": \"" +
                                    StateService.GetValue<string>(WebUiConstants.InitialQueryString)
                                    + "\", \"SessionId\": \"" + StateService.GetValue<string>(WebUiConstants.SessionId)
                                    + "\", \"FirstCookies\": {" +
                                    StateService.GetValue<string>(WebUiConstants.FirstCookies) + "}}"
                        );
                    }
                    logger.Info("SessionStart ss.EndUpWith: " + ss.ToString());

                }
                catch (Exception exception)
                {
                    logger.Fatal("SessionStart Exception!!! " + HttpContext.Current.Request.Url, exception);
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

        private void Session_End(object sender, EventArgs e)
        {
        }
    }
}
