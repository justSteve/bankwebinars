
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Notification.Email
{
    [Serializable]
    public class ConfirmAdditionalLocationsOrderMessage : IConfirmAdditionalLocationsOrderMessage
    {
        public IEnumerable<AdditionalLocationOrderDetailsMessage> DetailsOfAdditionalLocationsInOrder { get; set; }
    }
}
