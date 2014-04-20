using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Membership.Email;
using CUWebinars.Web.Services;

namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService, IRefDataRepository refDataRepository)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            //  
            var appinfo = new AspNetApplicationInformation(
                "CUWebinars",
                "TTS Staff",
                "Account/Login",
                "Account/Confirm/",
                "Account/RegisterCancel/",
                "Account/PasswordResetConfirm/");

            var delivery = new TtsSmtpMessageDelivery();
            var emailFormatter = new TtsEmailFormatter(appinfo, stateService, refDataRepository) { PathToRoot = pathToRootDirectory };

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}