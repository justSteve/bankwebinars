
namespace CUWebinars.Business.Core
{
    public struct PricesAndDiscounts
    {
        public decimal FlatOff { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalOptions { get; set; }
        public decimal TotalOrderPrice { get; set; }
    }
}