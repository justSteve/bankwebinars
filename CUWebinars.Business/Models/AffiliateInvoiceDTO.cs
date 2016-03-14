using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public class AffiliateInvoiceDTO
    {
        public AffiliateInvoiceDTO()
        {
            Orders = new List<Order>();
            Rows = new List<OrderRowModel>();
        }

        public int WebinarID { get; set; }
        
        public String AffiliateName { get; set; }
        public String WebinarTitle { get; set; }
        public Affiliate Affiliate { get; set; }
        public IList<Order> Orders { get; set; }
        public IList<OrderRowModel> Rows { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal TotalOnBilled { get; set; }//DueOnBilled
        public decimal TotalOnPaid { get; set; }
        public decimal TotalRoyalties { get; set; }//RoyaltyOnPaid
        public decimal TotalNetDue { get; set; }

        public string InvoiceId { get; set; }
        public string InoviceDate { get; set; }
        public string WebinarDate { get; set; }
    }
}
