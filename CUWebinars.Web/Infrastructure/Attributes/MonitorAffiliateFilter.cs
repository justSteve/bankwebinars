using CUWebinars.Business.Core;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace CUWebinars.Web.Infrastructure.Attributes
{
    public class MonitorAffiliateFilter : IActionFilter
    {
        private const char DotCharSeparator = '.';
        private const string ServerName = "SERVER_NAME";
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;

        public MonitorAffiliateFilter(IOrderManagementService orderManagementService, IStateService stateService)
        {
            _stateService = stateService;
            _orderManagementService = orderManagementService;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            HttpRequestBase currentRequest = filterContext.RequestContext.HttpContext.Request;
            var currentHost = currentRequest.ServerVariables[ServerName].Split(DotCharSeparator)[0];
            var subdomainBranding = _stateService.GetValue<string>(WebUiConstants.SubdomainBranding);


            if (!string.IsNullOrEmpty(currentRequest.QueryString[WebUiConstants.AffiliateId]))
            {
                /* ATTEMPT 1: Handle the affiliate ID comming from the ?idAff query string */
                ExtractFromQueryString(currentRequest);
            }
            else if (!string.IsNullOrEmpty(subdomainBranding))
            {
                /* ATTEMPT 2: we are running with an affiliate's subdomain */
                ExtractFromDomain(subdomainBranding);
            }
            else
            {
                /* ATTEMPT 3: assign the Affiliate based on business rules regarding historical behaviour */

            }

            
        }

        
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Not Implemented by Design. Not an Omission.
        }

        private void ExtractFromDomain(string subdomainBranding)
        {
            var subdomainBrandType = subdomainBranding.Split(DotCharSeparator).FirstOrDefault();

            if (!string.IsNullOrEmpty(subdomainBrandType) && subdomainBrandType.Equals(WebUiConstants.Webinars, StringComparison.OrdinalIgnoreCase))
            {
                //  we discover we are running with an affiliate's subdomain
                string affilliateDomain = @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL].Split(DotCharSeparator)[1];

                try
                {
                    _stateService.SetValue("AffiliateSessionSource", string.Concat("Sub | ", affilliateDomain));
                    //todo: 4BW - unit test required 
                    //   the name of the property 'ttsDomain' is the abbreviated name chosen
                    //   for use (as a shortcut or nicname) by us to refer to a given affiliate. It may or may not
                    //   be literally the Domain Name used by the given affiliate.

                    _stateService.SetValue(
                        WebUiConstants.CurrentAffiliate, 
                        _orderManagementService.GetAffiliateByDomain(affilliateDomain) ??_orderManagementService.GetAffiliateByDomain("bennett")
                        );
                }
                catch (Exception exception)
                {
                    ILog logger = LogManager.GetLogger(typeof (MonitorAffiliateFilter));
                    logger.Fatal(exception);
                    throw;
                }
            }
        }

        private void ExtractFromQueryString(HttpRequestBase currentRequest)
        {
            int loadAff;

            if (int.TryParse(currentRequest.QueryString[WebUiConstants.AffiliateId], out loadAff))
            {
                try
                {
                    _stateService.SetValue(
                        WebUiConstants.CurrentAffiliate,
                        _orderManagementService.GetAffiliateByIdLoaded(loadAff, a => a.WebUser)
                        );
                }
                catch (Exception exception)
                {
                    ILog logger = LogManager.GetLogger(typeof(MonitorAffiliateFilter));
                    logger.ErrorFormat("ERROR: failed to load idAff code: {0}", HttpContext.Current.Request.Url.ToString().Contains("idaff="));
                    logger.Error(exception.Message);
                    throw;
                }
            }
            else
            {
                ILog logger = LogManager.GetLogger(typeof(MonitorAffiliateFilter));
                logger.ErrorFormat("ERROR: MonitorAffiliateAttribute - Non - numberic idAff: {0}",
                    HttpContext.Current.Request.QueryString);
            }
        }
    }
}