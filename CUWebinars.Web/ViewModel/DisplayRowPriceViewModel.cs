
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayRowPriceViewModel
    {
        public int idOrder { get; set; }

        //public Discount Discount { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public string AddressesForAdditionalLocations { get; set; }
        public OrderStatus OrderStatus { get; set; }
        //public decimal Price { get; set; }
        //public decimal Tax { get; set; }
        public PricesAndDiscounts PricesAndDiscounts { get; set; }
        public RegType  RegistrationType { get; set; }
        public bool SendHardcopy { get; set; }
        public string Origin { get; set; }
        public bool WebinarIsPast { get; set; }

        //public decimal RowPrice { get; set; }
    }
}