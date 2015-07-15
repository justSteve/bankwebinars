using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Email
{
    public class AdhocNotificationMessage : IAdhocNotificationMessage
    {
        public string Body { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> Recipients { get; set; }
    }
}