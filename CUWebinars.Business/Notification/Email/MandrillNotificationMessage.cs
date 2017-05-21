using System.Collections.Generic;
using Mandrill.Model;

namespace CUWebinars.Business.Notification.Email
{
    public class MandrillNotificationMessage : IMandrillNotificationMessage
    {
        public string BaseUrl { get; set; }
        public string Body { get; set; }
        public string PersistedName { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> Recipients { get; set; }
        public MandrillMessage MandrillMessage { get; set; }
    }
}