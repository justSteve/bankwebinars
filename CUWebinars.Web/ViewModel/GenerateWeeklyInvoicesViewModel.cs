using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class GenerateWeeklyInvoicesViewModel
    {
        public IList<Webinar> Webinars { get; set; }
        public IList<Order> PosteventOrders { get; set; }
        public IList<Order> AdjustedtOrders { get; set; }
        public IList<Affiliate> Affiliates { get; set; }
    }
}