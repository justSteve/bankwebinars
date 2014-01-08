
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        public static MembershipRebootConfiguration Create()
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            //  TODO: get proper signature from Stephen
            var appinfo = new AspNetApplicationInformation("CUWebinars",
                "Test Email Signature",
                "Account/Login",
                "Account/PasswordResetConfirm/",
                "Account/Register/Cancel/",
                "Account/PasswordResetConfirm/");
 
            var emailFormatter = new EmailMessageFormatter(appinfo);

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}