using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class WebinarStatusUpdate
    {

        public IList<IncomingAdditionalLocation> AdditionalLocations { get; set; }
        public IList<Order> Orders { get; set; }

        public Webinar Webinar { get; set; }
        public string AffiliateComments { get; set; }


    }
}