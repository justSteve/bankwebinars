using System;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace CUWebinars.Web.Core
{
    public class GlobalConfig
    {

        public string EmailSendingMode { get; private set; }
        public string TestEmailAddress { get; private set; }
        public string WMVRepository { get; private set; }
        public string HandoutRepository { get; private set; }
        public string ImgRepository { get; private set; }
        public string Tenant { get; private set; }
        public string TenantEmail { get; private set; }
        public string TenantDomain { get; private set; }
        public string TenantURL { get; private set; }
        public string TenantLogo { get; private set; }
        public string TenantPrefix { get; private set; }

        public string MembershipConnectionString { get; set; }

        private GlobalConfig()
        {

        }

        internal class GlobalConfigSingletonCreator
        {
            static GlobalConfigSingletonCreator()
            {
                NameValueCollection ApplicationSettingsSection = WebConfigurationManager.AppSettings;
                
                //if(ReferenceEquals(null, ApplicationSettingsSection))
                //{
                //    throw new ArgumentNullException("AppSettings not found in config file as expected.");
                //}
                UniqueInstance.EmailSendingMode = ApplicationSettingsSection["EmailSendingMode"];
                UniqueInstance.TestEmailAddress = ApplicationSettingsSection["TestEmailAddress"];
                UniqueInstance.WMVRepository = ApplicationSettingsSection["WMVRepository"];
                UniqueInstance.HandoutRepository = ApplicationSettingsSection["HandoutRepository"];
                UniqueInstance.ImgRepository = ApplicationSettingsSection["ImgRepository"];
                UniqueInstance.Tenant = ApplicationSettingsSection["Tenant"];
                UniqueInstance.TenantEmail = ApplicationSettingsSection["TenantEmail"];
                UniqueInstance.TenantDomain = ApplicationSettingsSection["TenantDomain"];
                UniqueInstance.TenantURL = ApplicationSettingsSection["TenantURL"];
                UniqueInstance.TenantLogo = ApplicationSettingsSection["TenantLogo"];
                UniqueInstance.TenantPrefix = ApplicationSettingsSection["TenantPrefix"];

                UniqueInstance.MembershipConnectionString = WebConfigurationManager.ConnectionStrings["MembershipReboot"].ConnectionString;
            }

            // Private object instantiated with private constructor
            internal static readonly GlobalConfig UniqueInstance = new GlobalConfig();
        }

        // Public static property to get the singleton object
        public static GlobalConfig GlobalConfigSingleton
        {
            get
            {
                return GlobalConfigSingletonCreator.UniqueInstance;
            }
        }
    }
}