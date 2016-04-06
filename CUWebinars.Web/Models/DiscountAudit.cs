using System;

namespace CUWebinars.Web.Models
{
    public class DiscountAudit
    {
        public int idDiscount { get; set; }
        public string DiscountType { get; set; }
        public string DiscountCode { get; set; }
        public string PercentOff { get; set; }
        public string FlatOff { get; set; }
        public Decimal CreditsUsed { get; set; }
        public Decimal CreditsRemain { get; set; }
        public DateTime DateValidFrom { get; set; }
        public DateTime DateValidTo { get; set; }
        public string Status { get; set; }
        public DateTime Verified { get; set; }

    }
}