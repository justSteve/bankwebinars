
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Email;
using System.Web;
namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create(string pathToRootDirectory)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            //  TODO: get proper signature from Stephen
            var appinfo = new AspNetApplicationInformation(
                "CUWebinars",
                "Test Email Signature",
                "Account/Login",
                "Account/PasswordResetConfirm/",
                "Account/Register/Cancel/",
                "Account/PasswordResetConfirm/");

            var delivery = new TTSSmtpMessageDelivery();
            var emailFormatter = new TTSEmailFormatter(appinfo);
            emailFormatter.PathToRoot = pathToRootDirectory;

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}