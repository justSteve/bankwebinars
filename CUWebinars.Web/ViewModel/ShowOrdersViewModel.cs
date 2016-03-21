using System.Collections;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ShowOrdersViewModel
    {
        public bool UserIsAdmin { get; set; }
        public IList<Order> Orders { get; set; }
        public Affiliate Affiliate { get; set; }
        public Webinar Webinar { get; set; }
        public string SearchTerm { get; set; }
        public string NumOfOrders { get; set; }
    }
}