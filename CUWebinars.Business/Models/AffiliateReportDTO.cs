using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class AffiliateReportDTO
    {
        public AffiliateReportDTO()
        {
            Orders = new List<Order>();
        }

        public int WebinarID { get; set; }
        public Affiliate Affiliate { get; set; }
        public IList<Order> Orders { get; set; }
        public decimal TotalRevenues { get; set; }
        public decimal TotalCommissions { get; set; }
    }
}