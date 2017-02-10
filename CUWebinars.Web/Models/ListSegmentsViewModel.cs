using System.Collections.Generic;
using CUWebinars.Business.Models;
using MailChimp.Net.Models;

namespace CUWebinars.Web.Models
{
    public class ListSegmentsViewModel
    {
        public IList<Affiliate> NoList { get; set; }
        public IList<Affiliate> HaveSegment { get; set; }
        public IList<Dictionary<Affiliate, ListSegment>> AffAndListSegments { get; set; }
        public IList<Affiliate> HaveNoSegment { get; set; }
        public IList<ListSegment> ListSegments { get; set; }
    }
}