using System.Collections.Generic;

namespace CUWebinars.Business.Notification
{
    public interface INotificationMessage
    {
        string From { get; set; }
        string To { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        
        //  Note this property is an alternative to To, where the email is for multiple recipients
        IList<string> Addresses { get; set; }
    }
}