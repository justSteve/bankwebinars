using BrockAllen.MembershipReboot;

namespace CUWebinars.Business.Notification
{
    public interface IRazorRenderer<TAccount> where TAccount : UserAccount
    {
        string ConstructMessage<T>(Business.Notification.Events.UserAccountEvent<TAccount> accountEvent,
            EnvironmentInformation applicationInformation,
            string templatedText,
            T objectOfMessage
            );
    }
}