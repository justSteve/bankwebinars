using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationViewModel
    {
        public AddAdditionalLocationViewModel AddAdditionalLocationViewModel { get; set; }
        public IList<AdditionalLocation> AdditionalLocations { get; set; }

        public Order Order { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }

    }
}