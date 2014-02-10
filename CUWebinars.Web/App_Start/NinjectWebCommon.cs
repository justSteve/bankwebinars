using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Data.Repositories;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Services;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Web;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace CUWebinars.Web.App_Start
{

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
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var config = MembershipRebootConfig.Create(HttpRuntime.AppDomainAppPath);
            
            kernel.Bind<MembershipRebootConfiguration>().ToConstant(config);
            kernel.Bind<IAffiliateRepository>().To<AffiliateRepository>();
            kernel.Bind<IWebinarRepository>().To<WebinarRepository>();
            //kernel.Bind<IAccountRepository>().To<AccountRepository>().InRequestScope();
            //kernel.Bind<IPresenterRepository>().To<PresenterRepository>();
            kernel.Bind<TTSWebinarsContext>().To<TTSWebinarsContext>();
            kernel.Bind<IWebUserRepository>().To<WebUserRepository>();
            kernel.Bind<IOrderRepository>().To<OrderRepository>();
            kernel.Bind<IRefDataRepository>().To<RefDataRepository>();
            kernel.Bind<IInstitutionRepository>().To<InstitutionRepository>();
            kernel.Bind<IUserAccountRepository>().To<DefaultUserAccountRepository>();
            kernel.Bind<IOptionRepository>().To<OptionRepository>();
            kernel.Bind<IOrderManagementService>().To<OrderManagementService>();

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
