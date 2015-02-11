using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;

namespace CUWebinars.Web.Core
{
    public class GlobalConfig
    {

        public string CreateUserQueueName { get; private set; }
        public string EmailSendingMode { get; private set; }
        public string EmailSignature { get; private set; }
        public string HandoutRepository { get; private set; }
        public string ImgRepository { get; private set; }
        public string DefaultConnectionString { get; private set; }
        public int GhostRequestRetryLimit { get; private set; }
        public int GhostRequestTimeout { get; private set; }
        public string MembershipConnectionString { get; private set; }
        public bool NotificationsTesting { get; private set; }
        public string RelativeLoginUrl { get; private set; }
        public string RelativeConfirmChangeUrl { get; private set; }
        public string RelativeCancelVerificationUrl { get; private set; }
        public string RelativeConfirmPasswordResetUrl { get; private set; }
        public int RetryCount{ get; private set; }
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
        public string TraceLevel { get; set; }
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
                UniqueInstance.EmailSignature = ApplicationSettingsSection["EmailSignature"];
                //UniqueInstance.WMVRepository = ApplicationSettingsSection["WMVRepository"];
                UniqueInstance.HandoutRepository = ApplicationSettingsSection["HandoutRepository"];
                UniqueInstance.GhostRequestRetryLimit = int.Parse(ApplicationSettingsSection["GhostRequestRetryLimit"]);
                UniqueInstance.GhostRequestTimeout = int.Parse(ApplicationSettingsSection["GhostRequestTimeout"]);
                //UniqueInstance.ImgRepository = ApplicationSettingsSection["ImgRepository"];
                UniqueInstance.NotificationsTesting = bool.Parse(ApplicationSettingsSection["NotificationsTesting"]);
                UniqueInstance.RelativeLoginUrl = ApplicationSettingsSection["RelativeLoginUrl"];
                UniqueInstance.RelativeConfirmChangeUrl = ApplicationSettingsSection["RelativeConfirmChangeUrl"];
                UniqueInstance.RelativeCancelVerificationUrl = ApplicationSettingsSection["RelativeCancelVerificationUrl"];
                UniqueInstance.RelativeConfirmPasswordResetUrl = ApplicationSettingsSection["RelativeConfirmPasswordResetUrl"];
                UniqueInstance.RetryCount = int.Parse(ApplicationSettingsSection["RetryCount"]);
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
                UniqueInstance.TraceLevel = GetTraceLevel();
                //UniqueInstance.UnAuthenticatedUser = ApplicationSettingsSection["UnAuthenticatedUser"];
                UniqueInstance.UseAzureWebjobs = bool.Parse(ApplicationSettingsSection["UseAzureWebjobs"]);

                ConnectionStringSettingsCollection ConnectionStringSettings = WebConfigurationManager.ConnectionStrings;

                UniqueInstance.DefaultConnectionString = ConnectionStringSettings["DefaultConnection"].ConnectionString;
                UniqueInstance.MembershipConnectionString = ConnectionStringSettings["MembershipReboot"].ConnectionString;
            }

            private static string GetTraceLevel()
            {
                var diagnosticSection = WebConfigurationManager.GetSection("system.diagnostics") as ConfigurationSection;

                ConfigurationElementCollection sources =
                    diagnosticSection.ElementInformation.Properties["sources"].Value as ConfigurationElementCollection;

                Debug.Assert(sources != null, "Web.config must contain a Tracing section.");

                return (
                    from ConfigurationElement source in sources 
                    select source.ElementInformation.Properties["switchValue"].Value.ToString())
                    .FirstOrDefault();
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

        internal string PropertiesAsString
        {
            // nb: this property is internal to prevent StackOverflow exception. 
            // If public, it would recursively call itself ad finitem.
            // Iterator hits all public methods which are non-static.
            get
            {
                var valueNames = (from propertyInfo in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance) let value = propertyInfo.GetValue(GlobalConfigSingletonCreator.UniqueInstance) where value != null select string.Concat(propertyInfo.Name, ":", value.ToString())).ToList();

                return string.Join(";", valueNames);
            }
        }
    }
}