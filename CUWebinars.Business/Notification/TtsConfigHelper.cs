using System.Configuration;

namespace CUWebinars.Business.Notification
{
    public class TtsConfigHelper
    {
        public string GetEmailSendingMode()
        {
            return ConfigurationManager.AppSettings["EmailSendingMode"];
        }
    }
}
