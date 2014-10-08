
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayRowPriceViewModel
    {
        public Discount Discount { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public double Price { get; set; }
        public RegType  RegistrationType { get; set; }
        public decimal RowPrice { get; set; }
    }
}