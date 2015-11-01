using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOrdersViewModel
    {
        public IEnumerable<OrderSummary> OrderSummaries { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class OrderSummary
    {
        public int OrderId { get; set; }


        public string OrderColumn { get; set; }
        public string UserColumn { get; set; }
        public string InstitutionColumn { get; set; }
        public string BillingColumn { get; set; }
        public string DiscountColumn { get; set; }
        public string AffiliateColumn { get; set; }
        public string OrderDateColumn { get; set; }
        public string StatusColumn { get; set; }
    }
}