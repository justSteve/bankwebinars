
using System.Text;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class OrderSubmittedMultiViewModel
    {
        public string Subject { get; set; }
        public string OrderSummaryHtml { get; set; }
        public string DiscountCaption { get; set; }
        public string GrandTotalCaption { get; set; }
        public string HeaderSummaryCaption { get; set; }
    }
}
