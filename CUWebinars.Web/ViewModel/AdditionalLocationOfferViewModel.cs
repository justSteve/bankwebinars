using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationOfferViewModel
    {
        //public AdditionalLocation AdditionalLocation { get; set; }
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public AdditionalLocationAddViewModel AdditionalLocationAddViewModel { get; set; }
        public IList<string> Emails { get; set; } // IList type because need to access items via an indexer
        public bool OrderExists { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
        public decimal Price { get; set; }
    }
}