using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Web.Membership.Email;
using CUWebinars.Web.Services;


namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            var appinfo = new AspNetApplicationInformation(
                "CUWebinars",
                "TTS Staff",
                "Account/Login",
                "Account/Confirmed/",
                "Account/RegisterCancel/",
                "Account/PasswordResetConfirm/");

            //var delivery = new AzureWebJobSmtpMessageDelivery(stateService);
            var delivery = new TtsSmtpMessageDelivery(stateService);
            var emailFormatter = new TtsEmailFormatter(appinfo, stateService) { PathToRoot = pathToRootDirectory };

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}
