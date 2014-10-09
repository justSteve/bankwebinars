using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOptionsViewModel
    {
        public string EventTitle { get; set; }
        public int idWebinar { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }
        public IEnumerable<RegType> Options { get; set; }
        public bool OrderRowExists { get; set; }
        public decimal WebinarDuration { get; set; }
        public WebinarStatus WebinarStatus { get; set; }
    }
}