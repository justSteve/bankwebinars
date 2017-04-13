using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "First")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last")]
        public string LastName { get; set; }
        
        [Required]
        public string Institution { get; set; }
        
        [UIHint("Address")]
        public AddressModel BillingAddress { get; set; }
        
        [UIHint("Address")]
        public AddressModel ShippingAddress { get; set; }

        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [Display(Name = "TimeZone")]
        public USTimeZone TimeZone { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public UserType UserType { get; set; }
        [Required]
        public string Title { get; set; }
        public int? idWebUser { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }
        
    }
}