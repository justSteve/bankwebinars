using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOptionsViewModel
    {
        public AdditionalLocationOfferViewModel AdditionalLocationOfferViewModel { get; set; }
        public string EventTitle { get; set; }
        public int idWebinar { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }
        public IDictionary<RegType, bool> Options { get; set; }
        public bool OrderRowExists { get; set; }
        public decimal WebinarDuration { get; set; }
        public WebinarStatus WebinarStatus { get; set; }
    }
}