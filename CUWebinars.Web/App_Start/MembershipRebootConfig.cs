using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Web.Core;
using CUWebinars.Web.Membership.Email;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging.Log4net.Infrastructure;


namespace CUWebinars.Web.App_Start
{
    public class MembershipRebootConfig
    {
        private static readonly GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        public static MembershipRebootConfiguration Create(string pathToRootDirectory, IStateService stateService)
        {
            var settings = SecuritySettings.FromConfiguration();
            var config = new MembershipRebootConfiguration(settings);

            var appinfo = new AspNetApplicationInformation(
                _globalConfig.Tenant,
                _globalConfig.EmailSignature,
                _globalConfig.RelativeLoginUrl,
                _globalConfig.RelativeConfirmChangeUrl,
                _globalConfig.RelativeCancelVerificationUrl,
                _globalConfig.RelativeConfirmPasswordResetUrl);

            IMessageDelivery delivery;

            if(GlobalConfig.GlobalConfigSingleton.UseAzureWebjobs)
                delivery = new AzureWebJobSmtpMessageDelivery(stateService, new Log4NetLogger(typeof(AzureWebJobSmtpMessageDelivery)));
            else
                delivery = new TtsSmtpMessageDelivery(stateService);
                
            var emailFormatter = new TtsEmailFormatter(appinfo, stateService) { PathToRoot = pathToRootDirectory };

            // uncomment if you want email notifications -- also update smtp settings in web.config
            config.AddEventHandler(new EmailAccountEventsHandler(emailFormatter, delivery));
            //config.AddEventHandler(new TwilloSmsEventHandler(appinfo));

            return config;
        }
    }
}
