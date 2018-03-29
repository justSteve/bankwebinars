using System.Security.Claims;
using CUWebinars.Business.Core;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace CUWebinars.Web.Infrastructure.Attributes
{

    //This class was originally a way to Assign Affiliate
    // but was depricated and is not being hit any longer
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
            //depricated method of affiliate sourcing is no longer used.
            //var currentHost = currentRequest.ServerVariables[ServerName].Split(DotCharSeparator)[0];
            //var subdomainBranding = _stateService.GetValue<string>(WebUiConstants.SubdomainBranding);
            //var subdomainBranding = string.Empty;


            if (!string.IsNullOrEmpty(currentRequest.QueryString[WebUiConstants.AffiliateId]))
            {
                /* ATTEMPT 1: Handle the affiliate ID comming from the 
                 * query string */
                ExtractFromQueryString(currentRequest);
            }
            else
            {
                /* ATTEMPT 3: assign the Affiliate based on 
                 * business rules regarding historical behaviour */
                ExtractFromHistoricalUsageOfLoggedInUser(filterContext);
            }
        }

        private void ExtractFromHistoricalUsageOfLoggedInUser(ActionExecutingContext filterContext)
        {
            if (!ReferenceEquals(filterContext.HttpContext.User, null))
            {
                var userIdentity = filterContext.HttpContext.User.Identity;

                if (userIdentity.IsAuthenticated)
                {
                    var webUser =
                        _orderManagementService.GetWebUser(
                            ((ClaimsIdentity)userIdentity).Claims.Single(c => c.Type == ClaimTypes.Email).Value
                            );

                    Affiliate affiliate = null;
                    if (webUser != null)
                    {
                        //affiliate = _orderManagementService.DetermineAffiliateByAlternativeMeans(webUser.idUser, 0);

                    }
                    else
                    {
                        //hardwire a valid affiliate ID
                        //affiliate = _orderManagementService.DetermineAffiliateByAlternativeMeans(19, 0);
                    }

                    // if null returned, just use whatever is stored in Session for CurrentAffiliate. O/w, set that value.
                    if (!ReferenceEquals(null, affiliate))
                    {
                        _stateService.SetValue(WebUiConstants.CurrentAffiliate, affiliate);
                    }

                }
            }
            else
            {
                //HeyGeorge:
                //hardwires 'house' affiliate
                // re: affiliate id = 2988
                // both 19 and 2988 are 'magic Ids' that point to TTS affiliate accounts. 
                // Consider them to be interchangeable.
                //var affiliate = _orderManagementService.DetermineAffiliateByAlternativeMeans(2988, 0);
                //_stateService.SetValue(WebUiConstants.CurrentAffiliate, affiliate);
            }
        }


        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // Not Implemented by Design. Not an Omission.
        }

        //depricated method of affiliate sourcing is no longer used.
        //private void ExtractFromDomain(string subdomainBranding)
        //{
        //    var subdomainBrandType = subdomainBranding.Split(DotCharSeparator).FirstOrDefault();

        //    if (!string.IsNullOrEmpty(subdomainBrandType) && subdomainBrandType.Equals(WebUiConstants.Webinars, StringComparison.OrdinalIgnoreCase))
        //    {
        //        //  we discover we are running with an affiliate's subdomain
        //        string affilliateDomain = @System.Configuration.ConfigurationManager.AppSettings[AppConst.TESTING_URL].Split(DotCharSeparator)[1];

        //        try
        //        {
        //            _stateService.SetValue("AffiliateSessionSource", string.Concat("Sub | ", affilliateDomain));
        //            //   the name of the property 'ttsDomain' is the abbreviated name chosen
        //            //   for use (as a shortcut or nicname) by us to refer to a given affiliate. It may or may not
        //            //   be literally the Domain Name used by the given affiliate.

        //            _stateService.SetValue(
        //                WebUiConstants.CurrentAffiliate,
        //                _orderManagementService.GetAffiliateByDomain(affilliateDomain) ?? _orderManagementService.GetAffiliateByDomain("bennett")
        //                );
        //        }
        //        catch (Exception exception)
        //        {
        //            ILog logger = LogManager.GetLogger(typeof(MonitorAffiliateFilter));
        //            logger.Fatal(exception);
        //            throw;
        //        }
        //    }
        //}

        private void ExtractFromQueryString(HttpRequestBase currentRequest)
        {
            int loadAff;

            if (int.TryParse(currentRequest.QueryString[WebUiConstants.AffiliateId], out loadAff))
            {
                Affiliate foundAff =
                    _orderManagementService.GetAffiliateByIdLoaded(loadAff, a => a.WebUser);

                if (!ReferenceEquals(foundAff, null))
                {
                    _stateService.SetValue(
                        WebUiConstants.CurrentAffiliate,
                        foundAff
                        );
                }
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
                    logger.ErrorFormat("ERROR: failed to load idAff code: {0}", HttpContext.Current.Request.Url);
                    logger.Error(exception.Message);
                    _stateService.SetValue(
                        WebUiConstants.CurrentAffiliate,
                        _orderManagementService.GetAffiliateByIdLoaded(19, a => a.WebUser)
                        );

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