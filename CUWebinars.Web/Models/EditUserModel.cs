using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditUserModel
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

        public UserType UserType { get; set; }
        public string Title { get; set; }
        public int? idWebUser { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }


    }
}