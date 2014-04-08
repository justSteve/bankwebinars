using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOptionsViewModel
    {
        public AdditionalLocationViewModel AdditionalLocationViewModel { get; set; }
        public List<RegType> Options { get; set; }

        public Order Order { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
    }
}