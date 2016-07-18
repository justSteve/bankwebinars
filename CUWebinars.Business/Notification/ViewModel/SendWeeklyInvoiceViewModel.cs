using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class SendWeeklyInvoiceViewModel
    {
        public Affiliate Affiliate { get; set; } // really just need id and email
        public string InvoiceId { get; set; }
        public string DateRange { get; set; }
        public string InvoiceStorageUri { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
    }
}
