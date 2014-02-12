namespace CUWebinars.Business.Notification
{
    public interface INotificationMessage
    {
        string From { get; set; }
        string To { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
    }
}