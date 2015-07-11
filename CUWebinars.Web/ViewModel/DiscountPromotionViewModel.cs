using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.ViewModel
{
    public class DiscountPromotionViewModel
    {
        public string DiscountCode { get; set; }
        public decimal PercentOff { get; set; }
        public decimal FlatOff { get; set; }
        public decimal CreditsUsed { get; set; }
        public DateTime DateValidFrom { get; set; }
        public DateTime DateValidTo { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}