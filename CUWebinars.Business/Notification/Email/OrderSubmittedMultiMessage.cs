using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Email
{
    public class OrderSubmittedMultiMessage : IOrderSubmittedMultiMessage
    {
        public string BaseUrl { get; set; }
        public string Body { get; set; }
        public string PersistedName { get; set; }
        public string Subject { get; set; }
        public IEnumerable<string> Recipients { get; set; }
    }
}