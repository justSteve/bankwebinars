using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.ViewModel
{
    public class ResendOrderConfirmationViewModel
    {
        [Required]
        [Display(Name = "Order Id")]
        public string OrderId { get; set; }
    }
}