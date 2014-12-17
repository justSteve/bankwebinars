using CUWebinars.Business.Notification.Handlers;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.NotificationSystem.Event;

namespace CUWebinars.Business.Notification.Events
{
    public class OrderSubmittedAdditionalLocationEvent<T> : TtsBusEvent<T>, IAllowMultiple
        where T : OrderSubmittedAdditionalLocationViewModel
    {
        //TODO: not clear on the role played by the classes here and shared in this node.
        //   hoping for an overview of how this factors into the big picture.
        public string RelativeFilePath { get; set; }
    }
}