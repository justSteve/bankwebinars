
namespace CUWebinars.Business.Notification.Formatters
{
    public interface IFormatter
    {
        INotificationMessage Format<T>(T objectOfMessage, string templateName);
        string FormatToString<T>(T objectOfMessage, string templateName);
    }
}
