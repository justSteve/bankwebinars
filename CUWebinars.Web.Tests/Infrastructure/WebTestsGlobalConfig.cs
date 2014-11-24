using System.Collections.Specialized;
using System.Configuration;

namespace CUWebinars.Web.Tests.Infrastructure
{
    public class WebTestsGlobalConfig
    {
            private WebTestsGlobalConfig()
            {

            }

            internal class GlobalConfigSingletonCreator
            {
                static GlobalConfigSingletonCreator()
                {
                    NameValueCollection ApplicationSettingsSection = ConfigurationManager.AppSettings;

                    UniqueInstance.LoggedInUserEmail = ApplicationSettingsSection["LoggedInUserEmail"];
                    UniqueInstance.LoggedInUserPassword = ApplicationSettingsSection["LoggedInUserPassword"];
                    UniqueInstance.Tenant = ApplicationSettingsSection["Tenant"];

                    UniqueInstance.MembershipConnectionString = ConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString;
                }

                // Private object instantiated with private constructor
                internal static readonly WebTestsGlobalConfig UniqueInstance = new WebTestsGlobalConfig();
            }

        public string Tenant { get; private set; }

        public string LoggedInUserPassword { get; private set; }

        public string LoggedInUserEmail { get; private set; }

            public string MembershipConnectionString { get; private set; }

            // Public static property to get the singleton object
            public static WebTestsGlobalConfig WebTestsGlobalConfigSingleton
            {
                get
                {
                    return GlobalConfigSingletonCreator.UniqueInstance;
                }
            }
        }
}
