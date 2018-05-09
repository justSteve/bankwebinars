using System.Collections.Generic;
using CUWebinars.Business.Models;
using MailChimp.Net.Models;

namespace CUWebinars.Web.Models
{
    public class ListSegmentsViewModel
    {
        public IList<Affiliate> SBA { get; set; }
        public IList<Affiliate> CFT { get; set; }
        public IList<Affiliate> Other { get; set; }
        public string AffOutput { get; set; }

    }
}