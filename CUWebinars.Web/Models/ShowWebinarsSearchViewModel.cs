using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class ShowWebinarsSearchViewModel
    {
        public bool UserIsAdmin { get; set; }
        public IList<Order> Orders { get; set; }
        public Affiliate Affiliate { get; set; }
        public IList<Webinar> Webinars { get; set; }
        public IList<Topic> Topics { get; set; }
        public string SearchTerm  { get; set; }
    }
}
