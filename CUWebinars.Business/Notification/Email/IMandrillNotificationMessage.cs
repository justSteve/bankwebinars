using System.Collections.Generic;
using Mandrill.Model;

namespace CUWebinars.Business.Notification.Email
{
    public interface IMandrillNotificationMessage
    {
        string BaseUrl { get; set; }
        MandrillMessage MandrillMessage { get; set; }
        string Body { get; set; }
        string PersistedName { get; set; }
        string Subject { get; set; }
        IEnumerable<string> Recipients { get; set; }
    }
}