
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core
{
    public struct PricesAndDiscounts
    {
        public decimal FlatOff { get; set; }
        public Discount Discount { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        //renamed to help
        public decimal TotalCostOfOptions { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public decimal TaxAmount { get; set; }
        //public decimal OutstandingBalance { get; set; }
    }
}