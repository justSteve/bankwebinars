using BrockAllen.MembershipReboot;
using SmtpMessageDelivery = CUWebinars.Business.Notification.Email.SmtpMessageDelivery;

namespace CUWebinars.Business.Notification
{
    public class NotifyEventHandler<TAccount>
        where TAccount : UserAccount
    {
        private readonly INotificationFormatter<TAccount> _notificationFormatter;
        private readonly INotificationDelivery _notificationDelivery;

        public NotifyEventHandler(INotificationFormatter<TAccount> notificationFormatter)
            : this(notificationFormatter, new SmtpMessageDelivery())
        {
            _notificationFormatter = notificationFormatter;
        }

        public NotifyEventHandler(INotificationFormatter<TAccount> notificationFormatter, INotificationDelivery notificationDelivery)
        {
            _notificationFormatter = notificationFormatter;
            _notificationDelivery = notificationDelivery;
        }

        public virtual void Process<TBody>(Business.Notification.Events.UserAccountEvent<TAccount> evt, TBody objectOfMessage)
        {
           var notificationMessage = _notificationFormatter.Format(evt, objectOfMessage);

            _notificationDelivery.Notify(notificationMessage);
        }
    }
}
