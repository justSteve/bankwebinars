using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Email
{
    public interface IRecordingIsPosted2Message
    {
        string BaseUrl { get; set; }
        string Body { get; set; }
        string PersistedName { get; set; }
        string Subject { get; set; }
        IEnumerable<string> Recipients { get; set; }
    }
}