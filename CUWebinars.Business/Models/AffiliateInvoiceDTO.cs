using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public class AffiliateInvoiceDTO
    {
        public AffiliateInvoiceDTO()
        {
            Orders = new List<Order>();
        }

        public int WebinarID { get; set; }
        
        public String AffiliateName { get; set; }
        public String WebinarTitle { get; set; }
        public Affiliate Affiliate { get; set; }
        public IList<Order> Orders { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal TotalOnBilled { get; set; }//DueOnBilled
        public decimal TotalOnPaid { get; set; }
        public decimal TotalRoyalties { get; set; }//RoyaltyOnPaid
        public decimal TotalNetDue { get; set; }

        public string WebinarDate { get; set; }
        public int RowNumber { get; set; }
    }
}
