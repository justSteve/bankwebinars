using System.Collections.Generic;
using CUWebinars.Business.Notification.Email;

namespace CUWebinars.Business.Notification
{
    public interface IConfirmAdditionalLocationsOrderMessage
    {
        IEnumerable<AdditionalLocationOrderDetailsMessage> DetailsOfAdditionalLocationsInOrder { get; set; }
    }
}