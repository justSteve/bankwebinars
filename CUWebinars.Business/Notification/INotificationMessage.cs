using System.Collections.Generic;

namespace CUWebinars.Business.Notification
{
    public interface INotificationMessage
    {
        string From { get; set; }
        string To { get; set; }
        string CC { get; set; }
        string Bcc { get; set; }
        string ReplyTo { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        string PersistedName { get; set; }
        
        //  Note this property is an alternative to To, where the email is for multiple recipients
        IList<string> Addresses { get; set; }
    }
}