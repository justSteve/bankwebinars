using System;
using System.Collections.Specialized;
using System.Configuration;

namespace CUWebinars.WebUi.Tests2.Infrastructure
{
    public class WebUiTestGlobals
    {
        public string ChromeWebDriverPath { get; set; }
        public string ChromeWebDriverPort { get; set; }
        public string DefaultConnection { get; set; }
        public string FirefoxExePath { get; set; }
        public string FirefoxBrowserPort { get; set; }
        public string HomeUrl { get; set; }
        public string IeWebDriverPath { get; set; }
        public string IeWebDriverPort { get; set; }
        public string MembershipNotificationsUrl { get; set; }
        public string MembershipRebootConnection { get; set; }
        public string Tenant { get; set; }

        public string TtsDatabaseConnectionString { get; set; }

        internal class GlobalSingletonCreator
        {
            static GlobalSingletonCreator()
            {
                NameValueCollection applicationSettingsSection = ConfigurationManager.AppSettings;
                ConnectionStringSettingsCollection connectionStringSettingsSection = ConfigurationManager.ConnectionStrings;

                if (ReferenceEquals(null, applicationSettingsSection))
                {
                    throw new ArgumentNullException("AppSettings not found in config file as expected.");
                }

                UniqueInstance.IeWebDriverPath = applicationSettingsSection["IeWebDriverPath"];
                UniqueInstance.IeWebDriverPort = applicationSettingsSection["IeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPort = applicationSettingsSection["ChromeWebDriverPort"];
                UniqueInstance.ChromeWebDriverPath = applicationSettingsSection["ChromeWebDriverPath"];
                //UniqueInstance.DefaultConnection = connectionStringSettingsSection["DefaultConnection"].ConnectionString;
                UniqueInstance.FirefoxExePath = applicationSettingsSection["FirefoxExePath"];
                UniqueInstance.FirefoxBrowserPort = applicationSettingsSection["FirefoxBrowserPort"];
                UniqueInstance.HomeUrl = applicationSettingsSection["HomeUrl"];
                //UniqueInstance.MembershipNotificationsUrl = applicationSettingsSection["MembershipNotificationsUrl"];
                //UniqueInstance.MembershipRebootConnection = connectionStringSettingsSection["MembershipReboot"].ConnectionString;
                //UniqueInstance.Tenant = applicationSettingsSection["Tenant"];
                //UniqueInstance.TtsDatabaseConnectionString = connectionStringSettingsSection["TTSDataBase"].ConnectionString;
            }

            // Private object instantiated with private constructor
            internal static readonly WebUiTestGlobals UniqueInstance = new WebUiTestGlobals();
        }

        public static WebUiTestGlobals WebUiTestGlobalsConfigSingleton
        {
            get
            {
                return GlobalSingletonCreator.UniqueInstance;
            }
        }
    }
}
