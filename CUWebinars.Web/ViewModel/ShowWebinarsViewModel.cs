using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ShowWebinarsViewModel
    {
        public bool UserIsAdmin { get; set; }
        public IList<Order> Orders { get; set; }
        public Affiliate Affiliate { get; set; }
        public IList<Webinar> Webinars { get; set; }
        public IList<Presenter> Presenters { get; set; }
        public string SearchTerm { get; set; }
    }
}