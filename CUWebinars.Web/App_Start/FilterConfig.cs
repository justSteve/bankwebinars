using System.Web.Http;
using System.Web.Mvc;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Web.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var sc = new TTSWebinarsContext();
            var _orderManagementService = new OrderManagementService(new AffiliateRepository(sc),
                new RegTypeRepository(sc),
                new OrderRepository(sc),
                new RefDataRepository(),
                new WebUserRepository(sc),
                new WebinarRepository(sc),
                new AdditionalLocationRepository(sc),
                new Log4NetLogger(typeof(OrderManagementService)),
                (TtsConfiguration)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(TtsConfiguration))
                );
            var monitorAffiliateFilter = new MonitorAffiliateFilter(_orderManagementService, (IStateService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IStateService)));

            filters.Add(monitorAffiliateFilter);
            filters.Add(new HandleErrorAttribute());
        }
    }
}
