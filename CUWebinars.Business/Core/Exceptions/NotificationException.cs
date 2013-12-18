using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class NotificationException : TTSException
    {
        public NotificationException() :
            base()
        {
        }

        public NotificationException(string message)
            : base(message)
        {
        }

        public NotificationException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public static string BuildFatalErrorMessage(int templateID)
        {
            return String.Format(NOTIFICATION_FATAL_ERROR, templateID);
        }

        private const string NOTIFICATION_FATAL_ERROR = "Fatal error while processing template with ID = {0}. For more information look at the inner exception.";
    }
}
