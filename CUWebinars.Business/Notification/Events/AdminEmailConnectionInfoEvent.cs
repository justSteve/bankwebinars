using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Events
{
    public class AdminEmailConnectionInfoEvent<T> : AdminEmailEvent<T>
        where T : Order
    {
    }
}
