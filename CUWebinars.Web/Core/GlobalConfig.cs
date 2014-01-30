using System;
using System.Collections.Specialized;
using System.Web.Configuration;

namespace CUWebinars.Web.Core
{
    public class GlobalConfig
    {
        public string AppTenant { get; private set; }

        private GlobalConfig()
        {

        }

        private class GlobalConfigSingletonCreator
        {
            static GlobalConfigSingletonCreator()
            {
                NameValueCollection ApplicationSettingsSection = WebConfigurationManager.AppSettings;
                
                if(ReferenceEquals(null, ApplicationSettingsSection))
                {
                    throw new ArgumentNullException("AppSettings not found in config file as expected.");
                }

                uniqueInstance.AppTenant = ApplicationSettingsSection["Tenant"];
            }

            // Private object instantiated with private constructor
            internal static readonly GlobalConfig uniqueInstance = new GlobalConfig();
        }

        // Public static property to get the singleton object
        public static GlobalConfig GlobalConfigSingleton
        {
            get
            {
                return GlobalConfigSingletonCreator.uniqueInstance;
            }
        }
    }
}