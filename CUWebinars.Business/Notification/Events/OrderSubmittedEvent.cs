
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedEvent<TAccount> : UserAccountEvent<TAccount>
    {
        public Order Order { get; set; } 
    }
}