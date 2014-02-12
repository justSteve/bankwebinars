using BrockAllen.MembershipReboot;
using CUWebinars.Web.Notification;

namespace CUWebinars.Business.Notification
{
    public interface INotificationFormatter<TAccount>
        where TAccount : UserAccount
    {
        INotificationMessage Format<TBody>(Business.Notification.Events.UserAccountEvent<TAccount> userAccountEvent, TBody objectOfMessage);
    }
}
