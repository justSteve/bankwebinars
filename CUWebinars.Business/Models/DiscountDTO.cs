using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class DiscountDTO
    {
        public int idDiscount { get; set; }
        public int idAffiliate { get; set; }
        public DiscountType DiscountType { get; set; }

        public string DiscountCode { get; set; }
        public decimal PercentOff { get; set; }
        public decimal FlatOff { get; set; }
        public decimal CreditsUsed { get; set; }
        public decimal CreditsRemain { get; set; }
        public DateTime DateValidFrom { get; set; }
        public DateTime DateValidTo { get; set; }
        public string Status { get; set; }
        public DateTime? DateBilled { get; set; }
        public decimal? Cost { get; set; }
        public string Notes { get; set; }
        public int RenewalTerm { get; set; }
        public WebUserDiscountXref WebUserDiscountXref { get; set; }
        public object UserEmail { get; set; }
        public IList<WebUser> WpsUsers { get; set; }
    }
}
