using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.Models
{
    public class ShippingDetailsModel
    {
        public int UserId { get; set; }
        [UIHint("Address")]
        public AddressModel ShippingAddress { get; set; }
    }
}