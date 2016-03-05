using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Email
{
    public class NotificationMessage : INotificationMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string PersistedName { get; set; }
        public IList<string> Addresses { get; set; }
    }
}
