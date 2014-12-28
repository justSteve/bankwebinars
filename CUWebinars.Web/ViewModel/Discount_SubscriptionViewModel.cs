using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.ViewModel
{
    public class DiscountSubscriptionViewModel
    {

        public int idDiscount { get; set; }
        public string DiscountCode { get; set; }
        public DateTime DateValidFrom { get; set; }
        public DateTime DateValidTo { get; set; }
        public string Status { get; set; }
        public DateTime? DateBilled { get; set; }
        public string Notes { get; set; }
        public int RenewalTerm { get; set; }
    }
}