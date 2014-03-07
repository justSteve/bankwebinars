using System.Collections;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarDetailsViewModel
    {
        //public WebinarDetailsViewModel() 
        //{


        //}
        public string btnWebinar { get; set; }
        public string btnWhichState { get; set; }
        public string btnHasOrder { get; set; }
        public string stage_of_checkout { get; set; }


        public WebUser WebUser { get; set; }
        public Webinar Webinar { get; set; }
        public IList<Option> Options { get; set; }
        public Affiliate Affiliate { get; set; }
        public Order Order { get; set; }
        //public OrderRow OrderRow { get; set; }
        // ConnectionInfo is now a property of the Webinar entity
        //public string ConnectionInfo { get; set; }
    }
}