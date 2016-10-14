using System.Collections.Generic;

namespace CUWebinars.Business.Notification
{
    public interface IOrderSubmittedMultiMessage
    {
        string BaseUrl { get; set; }
        string Body { get; set; }
        string PersistedName { get; set; }
        string Subject { get; set; }
        IEnumerable<string> Recipients { get; set; }
    }
}