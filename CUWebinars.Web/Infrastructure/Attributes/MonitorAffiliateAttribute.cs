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
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class MonitorAffiliateAttribute : ActionFilterAttribute{}

    public class MonitorAffiliateFilter : IActionFilter
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IStateService _stateService;

        public MonitorAffiliateFilter(IOrderManagementService orderManagementService, IStateService stateService)
        {
            _stateService = stateService;
            _orderManagementService = orderManagementService;
        }

        //public MonitorAffiliateFilter(Func<IKernel> kernel)
        //{
        //    _kernel = kernel;
        //}

        public  void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase currentRequest = filterContext.RequestContext.HttpContext.Request;
            var currentHost = currentRequest.ServerVariables["SERVER_NAME"].Split('.')[0];
            var subdomainBranding = ""; // get from Web.config?


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

            /* ATTEMPT 3: get from an affiliate's subdomain */

            /*METHOD 3: VIA THE SUBDOMAIN -- implementation of this method is on hold pending SEO considerations.*/

            try
            {
                if (currentHost == "webinars")
                {
                    currentHost = currentRequest.ServerVariables["SERVER_NAME"].Split('.')[1];
                }

                //var affID = UserFacadeExtender.GetAffilliateByTTSDomain(currentHost);
                //if (affID > 0 && currentHost != "www")
                //{


                //    filterContext.HttpContext.Session[WebConst.AFFILIATE_SESSION_STORE] = affID;
                //    //}

                //    Logger.Instance.LogMessage("MonitorAffiliateAttribute assigns: " + affID);
                //}
            }
            catch (Exception ex)
            {

                //filterContext.HttpContext.Session[WebConst.AFFILIATE_SESSION_STORE] = AppConst.DEFAULT_AFFILIATE;
                //Logger.Instance.LogMessage("ERROR: MonitorAffiliateAttribute falls thru to default affiliate: " + ex);
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //throw new NotImplementedException();
        }

        private void ExtractFromDomain(string subdomainBranding)
        {
            var subdomainBrandType = subdomainBranding.Split('.').FirstOrDefault();

            if (!string.IsNullOrEmpty(subdomainBrandType) &&
                subdomainBrandType.Equals("webinars", StringComparison.OrdinalIgnoreCase))
            {
                //  we discover we are running with an affiliate's subdomain
                string affilliateDomain =
                    @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL].Split('.')[1];
                try
                {
                    _stateService.SetValue("AffiliateSessionSource", string.Concat("Sub | ", affilliateDomain));
                    //todo: 4BW - unit test required 
                    //   the name of the property 'ttsDomain' is the abbreviated name chosen
                    //   for use (as a shortcut or nicname) by us to refer to a given affiliate. It may or may not
                    //   be literally the Domain Name used by the given affiliate.

                    HttpContext.Current.Session[WebUiConstants.CurrentAffiliate] =
                        _orderManagementService.GetAffiliateByDomain(affilliateDomain) ??
                        _orderManagementService.GetAffiliateByDomain("bennett");
                }
                catch (Exception exception)
                {
                    ILog logger = LogManager.GetLogger(typeof (MonitorAffiliateAttribute));
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
                    ILog logger = LogManager.GetLogger(typeof (MonitorAffiliateAttribute));
                    logger.ErrorFormat("ERROR: failed to load idAff code: {0}",
                        HttpContext.Current.Request.Url.ToString().Contains("idaff="));
                    logger.Error(exception.Message);
                    throw;
                }
            }
            else
            {
                ILog logger = LogManager.GetLogger(typeof (MonitorAffiliateAttribute));
                logger.ErrorFormat("ERROR: MonitorAffiliateAttribute - Non - numberic idAff: {0}",
                    HttpContext.Current.Request.QueryString);
            }
        }
    }
}