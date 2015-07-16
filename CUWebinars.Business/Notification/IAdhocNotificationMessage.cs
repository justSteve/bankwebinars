using System.Collections.Generic;

namespace CUWebinars.Business.Notification
{
    public interface IAdhocNotificationMessage
    {
        string Body { get; set; }
        string Subject { get; set; }
        IEnumerable<string> Recipients { get; set; }
    }
}