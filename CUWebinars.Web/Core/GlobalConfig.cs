using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;

namespace CUWebinars.Web.Core
{
    public class GlobalConfig
    {

        public string CreateUserQueueName { get; private set; }
        public string EmailSendingMode { get; private set; }
        public string HandoutRepository { get; private set; }
        public string ImgRepository { get; private set; }
        public string DefaultConnectionString { get; private set; }
        public string MembershipConnectionString { get; private set; }
        public bool NotificationsTesting { get; private set; }
        public string StorageAccessKey { get; private set; }
        public string StorageAccountName { get; private set; }
        public string Tenant { get; private set; }
        public string TenantEmail { get; private set; }
        public string TenantDomain { get; private set; }
        public string TenantURL { get; private set; }
        public string TenantLogo { get; private set; }
        public string TenantPrefix { get; private set; }
        public string TestEmailAddress { get; private set; }
        public string TestEmailAddress2 { get; private set; }
        public string UnAuthenticatedUser { get; private set; }
        public bool UseAzureWebjobs { get; private set; }
        public string WMVRepository { get; private set; }

        private GlobalConfig()
        {

        }

        internal class GlobalConfigSingletonCreator
        {
            static GlobalConfigSingletonCreator()
            {
                NameValueCollection ApplicationSettingsSection = WebConfigurationManager.AppSettings;

                UniqueInstance.CreateUserQueueName = ApplicationSettingsSection["CreateUserQueueName"];
                UniqueInstance.EmailSendingMode = ApplicationSettingsSection["EmailSendingMode"];
                UniqueInstance.WMVRepository = ApplicationSettingsSection["WMVRepository"];
                UniqueInstance.HandoutRepository = ApplicationSettingsSection["HandoutRepository"];
                UniqueInstance.ImgRepository = ApplicationSettingsSection["ImgRepository"];
                UniqueInstance.NotificationsTesting = bool.Parse(ApplicationSettingsSection["NotificationsTesting"]);
                UniqueInstance.StorageAccessKey = ApplicationSettingsSection["StorageAccessKey"];
                UniqueInstance.StorageAccountName = ApplicationSettingsSection["StorageAccountName"];
                UniqueInstance.Tenant = ApplicationSettingsSection["Tenant"];
                UniqueInstance.TenantEmail = ApplicationSettingsSection["TenantEmail"];
                UniqueInstance.TenantDomain = ApplicationSettingsSection["TenantDomain"];
                UniqueInstance.TenantURL = ApplicationSettingsSection["TenantURL"];
                UniqueInstance.TenantLogo = ApplicationSettingsSection["TenantLogo"];
                UniqueInstance.TenantPrefix = ApplicationSettingsSection["TenantPrefix"];
                UniqueInstance.TestEmailAddress = ApplicationSettingsSection["TestEmailAddress"];
                UniqueInstance.TestEmailAddress2 = ApplicationSettingsSection["TestEmailAddress2"];
                UniqueInstance.UnAuthenticatedUser = ApplicationSettingsSection["UnAuthenticatedUser"];
                UniqueInstance.UseAzureWebjobs = bool.Parse(ApplicationSettingsSection["UseAzureWebjobs"]);

                ConnectionStringSettingsCollection ConnectionStringSettings = WebConfigurationManager.ConnectionStrings;

                UniqueInstance.DefaultConnectionString = ConnectionStringSettings["DefaultConnection"].ConnectionString;
                UniqueInstance.MembershipConnectionString = ConnectionStringSettings["MembershipReboot"].ConnectionString;
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