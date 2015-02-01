using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class AdminEmailSendShippedOrderEvent<T> : AdminEmailEvent<T>
        where T : Order
    {
        
    }
}
