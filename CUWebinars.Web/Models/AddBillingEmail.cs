using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class AddBillingEmailModel
    {
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "OrderID")]
        public int OrderId { get; set; }

        [HiddenInput]
        public bool EmailSent { get; set; }
    }
}