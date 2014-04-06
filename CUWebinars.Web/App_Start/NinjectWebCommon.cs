using System.Web.Http;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Services;
using Ninject.Parameters;
using Ninject.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(CUWebinars.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(CUWebinars.Web.App_Start.NinjectWebCommon), "Stop")]

namespace CUWebinars.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

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
            const string refdataRepository = "RefDataRepository";
            const string refDataRepositoryForTokenizer = "RefDataRepositoryForTokenizer";
            const string optionRepository = "OptionRepository";

            string baseUrl = HttpRuntime.AppDomainAppPath;
            kernel.Bind<IStateService>().To<StateService>();

            kernel.Bind<IRefDataRepository>().To<RefDataRepository>().InRequestScope().Named(refdataRepository);
            kernel.Bind<TTSWebinarsContext>().To<TTSWebinarsContext>().InTransientScope();

            var config = MembershipRebootConfig.Create(baseUrl, 
                kernel.Get<IStateService>(),
                kernel.Get<IRefDataRepository>(
                    refdataRepository, 
                    new Parameter("contextForTokenizer", kernel.Get<TTSWebinarsContext>(), true)
                    )
                );
            var ttsConfig = TtsConfig.Create(baseUrl);

            kernel.Bind<MembershipRebootConfiguration>().ToConstant(config);
            kernel.Bind<TtsConfiguration>().ToConstant(ttsConfig);

            kernel.Bind<IAffiliateRepository>().To<AffiliateRepository>().InRequestScope().Named(affiliateRepository);
            kernel.Bind<IWebinarRepository>().To<WebinarRepository>().InRequestScope().Named(webinarRepository);
            //kernel.Bind<IAccountRepository>().To<AccountRepository>().InRequestScope();
            //kernel.Bind<IPresenterRepository>().To<PresenterRepository>();
            kernel.Bind<IWebUserRepository>().To<WebUserRepository>().InRequestScope().Named(webuserRepository);
            kernel.Bind<IOrderRepository>().To<OrderRepository>().InRequestScope().Named(orderRepository);
            kernel.Bind<IInstitutionRepository>().To<InstitutionRepository>().InRequestScope();
            kernel.Bind<IUserAccountRepository>().To<DefaultUserAccountRepository>();
            kernel.Bind<IOptionRepository>().To<OptionRepository>().InRequestScope().Named(optionRepository);
            //kernel.Bind<IRefDataRepository>().To<RefDataRepository>().InRequestScope().Named(refdataRepository);
            kernel.Bind<IOrderManagementService>().ToMethod(ctx =>
            {
                var context = ctx.Kernel.Get<TTSWebinarsContext>();
                var param = new Parameter("context", context, true);

                var orderManagementService = new OrderManagementService(
                    ctx.Kernel.Get<IAffiliateRepository>(affiliateRepository, param),
                    ctx.Kernel.Get<IOptionRepository>(optionRepository, param),
                    ctx.Kernel.Get<IOrderRepository>(orderRepository, param),
                    ctx.Kernel.Get<IRefDataRepository>(refdataRepository, param),
                    ctx.Kernel.Get<IWebUserRepository>(webuserRepository, param),
                    ctx.Kernel.Get<IWebinarRepository>(webinarRepository, param),
                    ttsConfig
                    );
                return orderManagementService;

            }).InRequestScope();

            kernel.Bind<UserAccountService>().ToMethod(ctx =>
            {
                var userAccountService = new UserAccountService(config, ctx.Kernel.Get<IUserAccountRepository>());
                return userAccountService;
            });

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
