using System.Configuration;

namespace CUWebinars.Business.Notification
{
    public class TtsConfigHelper
    {
        public string GetEmailSendingMode()
        {
            return ConfigurationManager.AppSettings["EmailSendingMode"];
        }
        public string Tenant()
        {
            return ConfigurationManager.AppSettings["Tenant"];

        }
        public string TenantLogo()
        {
            return ConfigurationManager.AppSettings["TenantLogo"];

        }

        public string TenantEmail()
        {
            return ConfigurationManager.AppSettings["TenantEmail"];

        }

        public string TenantDomain()
        {
            return ConfigurationManager.AppSettings["TenantDomain"];

        }

        public string TenantURL()
        {
            return ConfigurationManager.AppSettings["TenantURL"];

        }
        public string TenantPrefix()
        {
            return ConfigurationManager.AppSettings["TenantPrefix"];
        }



    }
}
