
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayRowPriceViewModel
    {
        //public Discount Discount { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public string AddressesForAdditionalLocations { get; set; }
        public OrderStatus OrderStatus { get; set; }
        //public decimal Price { get; set; }
        //public decimal Tax { get; set; }
        public PricesAndDiscounts PricesAndDiscounts { get; set; }
        public RegType  RegistrationType { get; set; }
        //public decimal RowPrice { get; set; }
    }
}