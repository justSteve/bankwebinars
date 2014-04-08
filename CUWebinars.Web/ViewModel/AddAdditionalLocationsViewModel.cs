using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AddAdditionalLocationViewModel
    {
        //public IList<string> Emails { get; set; }
        public IList<AdditionalLocation> AdditionalLocations { get; set; }
        public Order Order { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }

    }
}