using System.Linq;
using System.Reflection;
using System.Web.Services.Description;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core;
using CUWebinars.Business.CQS;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System.Web.Http;

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
        private static readonly Assembly _serviceAssembly = Assembly.Load("CUWebinars.Business");

        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof (OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof (NinjectHttpModule));
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
            string baseUrl = HttpRuntime.AppDomainAppPath;

            kernel.Bind<IStateService>().To<StateService>().InRequestScope();
            kernel.Bind<IRefDataRepository>().To<RefDataRepository>().InRequestScope();
            kernel.Bind<TTSWebinarsContext>().ToSelf().InRequestScope();

            kernel.Bind<MembershipRebootConfiguration>().ToMethod(ctx =>
                MembershipRebootConfig.Create(baseUrl,
                    kernel.Get<IStateService>()
                    )).InRequestScope();


            kernel.Bind<TtsConfiguration>().ToMethod(ctx => TtsConfig.Create(baseUrl)).InRequestScope();

            kernel.Bind<IAffiliateRepository>().To<AffiliateRepository>().InRequestScope();
            kernel.Bind<IWebinarRepository>().To<WebinarRepository>().InRequestScope();
            kernel.Bind<IWebUserRepository>().To<WebUserRepository>().InRequestScope();

            //attempting to inject logger to WebUserRepository
            //kernel.Bind<IWebUserRepository>().ToMethod<WebUserRepository>(ctx =>
            //{
            //    ILogger loggerForWebUserRepository = new Log4NetLogger(typeof(WebUserRepository));
            //    return new WebUserRepository(
            //        loggerForWebUserRepository);
            //}).InRequestScope();


            kernel.Bind<IOrderRepository>().To<OrderRepository>().InRequestScope();
            kernel.Bind<IInstitutionRepository>().To<InstitutionRepository>().InRequestScope();
            kernel.Bind<IUserAccountRepository>().To<DefaultUserAccountRepository>().InRequestScope();
            kernel.Bind<IRegTypeRepository>().To<RegTypeRepository>().InRequestScope();

            kernel.Bind<IOrderManagementService>().ToMethod(ctx =>
            {
                var sharedContext = ctx.Kernel.Get<TTSWebinarsContext>();
                ILogger loggerForOrderManagementService = new Log4NetLogger(typeof (OrderManagementService));

                return new OrderManagementService(new AffiliateRepository(sharedContext),
                    new RegTypeRepository(sharedContext),
                    new OrderRepository(sharedContext),
                    new RefDataRepository(),
                    new WebUserRepository(sharedContext),
                    new WebinarRepository(sharedContext),
                    loggerForOrderManagementService,
                    ctx.Kernel.Get<TtsConfiguration>()
                    );
            }).InRequestScope();

            kernel.Bind<UserAccountService>().ToMethod(ctx =>
            {
                var userAccountService = new UserAccountService(
                    ctx.Kernel.Get<MembershipRebootConfiguration>(),
                    ctx.Kernel.Get<IUserAccountRepository>());
                return userAccountService;
            }).InRequestScope();

            kernel.Bind<AuthenticationService>().To<SamAuthenticationService>().InRequestScope();
            
            kernel.Bind<IMembershipService>().ToMethod(ctx =>
            {
                var sharedContext = ctx.Kernel.Get<TTSWebinarsContext>();
                var userAccountService = kernel.Get<UserAccountService>();
                ILogger loggerForMembershipService = new Log4NetLogger(typeof(MembershipService));


                return new MembershipService(
                    new InstitutionRepository(sharedContext),
                    new RefDataRepository(),
                    new SamAuthenticationService(userAccountService),
                    userAccountService,
                    new WebUserRepository(sharedContext),
                    loggerForMembershipService
                    );
            }).InRequestScope();

            kernel.Bind<ICommandProcessor>().To<CommandProcessor>().InRequestScope();
            kernel.Bind<IQueryProcessor>().To<QueryProcessor>().InRequestScope();
            AutoRegisterType(typeof(ICommandHandler<>), kernel); // Register ICommandHandler
            AutoRegisterType(typeof(IQueryHandler<,>), kernel); // Register IQueryHandler 

            kernel.Bind<IOrderControllerOrchestrator>().To<OrderControllerOrchestrator>().InRequestScope();

        }

        private static void AutoRegisterType(Type type, IKernel kernel)
        {
            var handlerRegistrations = _serviceAssembly.GetExportedTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => !x.ContainsGenericParameters)
                .SelectMany(x => x.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Where(i => i.GetGenericTypeDefinition() == type)
                    .Select(i => new {service = i, implementation = x})
                );

            foreach (var registration in handlerRegistrations)
            {
                var abstraction = registration.service; //  interface
                var implementation = registration.implementation; // class inplementation
                kernel.Bind(abstraction).To(implementation).InRequestScope();
            }
        }
    }
}
