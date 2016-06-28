using System.Collections.Generic;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class SendPromoLinksViewModel
    {
        public string Subject { get; set; }
        public List<string> FileLinks { get; set; }
    }
}
