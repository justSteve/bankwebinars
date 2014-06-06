using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Membership.Email;
using CUWebinars.Web.Services;
using Glimpse.Core.Extensibility;

using Ninject.Extensions.Logging;

namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService, IRefDataRepository refDataRepository)
        //public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService, IRefDataRepository refDataRepository, Ninject.Extensions.Logging.ILogger logger)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            //  
            var appinfo = new AspNetApplicationInformation(
                "CUWebinars",
                "TTS Staff",
                "Account/Login",
                "Account/Confirmed/",
                "Account/RegisterCancel/",
                "Account/PasswordResetConfirm/");

            //var delivery = new TtsSmtpMessageDelivery(stateService, logger);
            var delivery = new TtsSmtpMessageDelivery(stateService);
            var emailFormatter = new TtsEmailFormatter(appinfo, stateService, refDataRepository) { PathToRoot = pathToRootDirectory };

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}