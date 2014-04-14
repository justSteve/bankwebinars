using System.Web.Http;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Controllers.api;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using Ninject.Parameters;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CUWebinars.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CUWebinars.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CUWebinars.Web.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);

                GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            const string affiliateRepository = "AffiliateRepository";
            const string webinarRepository = "WebinarRepository";
            const string webuserRepository = "WebUserRepository";
            const string orderRepository = "OrderRepository";
            const string regTypeRepository = "RegTypeRepository";

            string baseUrl = HttpRuntime.AppDomainAppPath;
            kernel.Bind<IStateService>().To<StateService>();

            kernel.Bind<IRefDataRepository>().To<RefDataRepository>();

            var config = MembershipRebootConfig.Create(baseUrl, kernel.Get<IStateService>(), kernel.Get<IRefDataRepository>());
            var ttsConfig = TtsConfig.Create(baseUrl);

            kernel.Bind<MembershipRebootConfiguration>().ToConstant(config);
            kernel.Bind<TtsConfiguration>().ToConstant(ttsConfig);

            kernel.Bind<IAffiliateRepository>().To<AffiliateRepository>().InRequestScope().Named(affiliateRepository);
            kernel.Bind<IWebinarRepository>().To<WebinarRepository>().InRequestScope().Named(webinarRepository);
            kernel.Bind<IWebUserRepository>().To<WebUserRepository>().InRequestScope().Named(webuserRepository);
            kernel.Bind<IOrderRepository>().To<OrderRepository>().InRequestScope().Named(orderRepository);
            kernel.Bind<IInstitutionRepository>().To<InstitutionRepository>().InRequestScope();
            kernel.Bind<IUserAccountRepository>().To<DefaultUserAccountRepository>().InRequestScope();
            kernel.Bind<IRegTypeRepository>().To<RegTypeRepository>().InRequestScope().Named(regTypeRepository);

                        kernel.Bind<UserAccountService>().ToMethod(ctx =>
            {
                var userAccountService = new UserAccountService(config, ctx.Kernel.Get<IUserAccountRepository>());
                return userAccountService;
            });

            kernel.Bind<OrderController>().ToMethod(ctx =>
            {
                var sharedContext = new TTSWebinarsContext();
                var userAccountService = kernel.Get<UserAccountService>();

                var orderManagementService = new OrderManagementService(
                    new AffiliateRepository(sharedContext),
                    new RegTypeRepository(sharedContext),
                    new OrderRepository(sharedContext),
                    new RefDataRepository(),
                    new WebUserRepository(sharedContext),
                    new WebinarRepository(sharedContext),
                    ttsConfig
                    );

                var membershipService = new MembershipService(
                    new InstitutionRepository(sharedContext),
                    new RefDataRepository(),
                    new SamAuthenticationService(userAccountService),
                    userAccountService,
                    new WebUserRepository(sharedContext));

                return new OrderController(membershipService, orderManagementService, kernel.Get<IStateService>());

            }).InRequestScope();

            //kernel.Bind<IOrderManagementService>().ToMethod(ctx =>
            //{
            //    var orderManagementService = new OrderManagementService(
            //        new AffiliateRepository(sharedContext),
            //        new RegTypeRepository(sharedContext),
            //        new OrderRepository(sharedContext),
            //        new RefDataRepository(),
            //        new WebUserRepository(sharedContext),
            //        new WebinarRepository(sharedContext),
            //        ttsConfig
            //        );
            //    return orderManagementService;

            //}).InRequestScope();



            kernel.Bind<AuthenticationService>().To<SamAuthenticationService>();
            kernel.Bind<IMembershipService>().To<MembershipService>();
#if DEBUG
            kernel.Bind<IMailService>().To<MailServiceMock>();
#else
      kernel.Bind<IMailService>().To<MailService>().InRequestScope();
#endif
        }
    }
}
