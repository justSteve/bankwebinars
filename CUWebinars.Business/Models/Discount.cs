using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Discount
    {
        public int idDiscount { get; set; }
        public DiscountType discountType { get; set; }
        
        public string code { get; set; }
        public decimal percentOff { get; set; }
        public decimal flatOff { get; set; }
        public int usesNumber { get; set; }
        public System.DateTime dateValidFrom { get; set; }
        public System.DateTime dateValidTo { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> dateBilled { get; set; }
        public Nullable<decimal> cost { get; set; }
        public string Notes { get; set; }
    }
}
