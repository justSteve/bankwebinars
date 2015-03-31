using System.ComponentModel.DataAnnotations;
//modeled after ShippingDetailsModel
namespace CUWebinars.Web.Models
{
    public class DiscountDetailsModel
    {
        public int UserId { get; set; }
        [UIHint("Discount")]
        public DiscountModel Discount { get; set; }
    }
}