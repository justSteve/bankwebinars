using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarDetailsViewModel
    {
        public string btnWebinar { get; set; }
        public string btnWhichState { get; set; }
        public string btnHasOrder { get; set; }
        public string stage_of_checkout { get; set; }

        public Affiliate Affiliate { get; set; }
        public IList<RegType> Options { get; set; }
        public Order Order { get; set; }

        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
    }
}