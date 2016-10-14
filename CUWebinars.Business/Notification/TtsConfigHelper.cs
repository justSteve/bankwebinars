using System;
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

        public string OrderHasAdditionalLocations(int idOrder)
        {


            return null;

        }
        public string GetPromoEmailFromAddress()
        {
            return ConfigurationManager.AppSettings["PromoEmailFromAddress"];
        }
        public string GetPromoEmailSubject()
        {
            return ConfigurationManager.AppSettings["PromoEmailSubject"];
        }

        public string GetWeeklyInvoiceEmailFromAddress()
        {
            return ConfigurationManager.AppSettings["WeeklyInvoiceEmailFromAddress"];
        }
        public string GetWeeklyInvoiceEmailSubject()
        {
            return ConfigurationManager.AppSettings["WeeklyInvoiceEmailSubject"];
        }
        public string DiscountCreditUnitCost()
        {
            return ConfigurationManager.AppSettings["DiscountCreditUnitCost"];
        }
        public string GetOrderSubmittedMultiFromAddress()
        {
            return ConfigurationManager.AppSettings["OrderSubmittedMultiFromAddress"];
        }

        public string GetOrderSubmittedMultiEmailSubject()
        {
            return ConfigurationManager.AppSettings["OrderSubmittedMultiEmailSubject"];
        }

        public string GetCuwNotificationQueueName()
        {
            return ConfigurationManager.AppSettings["CuwNotificationQueueName"];
        }

        public string GetOrderSubmittedMultiQueueName()
        {
            return ConfigurationManager.AppSettings["OrderSubmittedMultiQueueName"];
        }
    }
}
