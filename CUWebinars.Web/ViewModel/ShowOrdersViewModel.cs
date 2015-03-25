using System.Collections;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ShowOrdersViewModel
    {
        public IList<Order> Orders { get; set; }
        public Affiliate Affiliate { get; set; }
        public Webinar Webinar { get; set; }
    }
}