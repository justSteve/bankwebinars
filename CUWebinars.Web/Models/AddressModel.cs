using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class AddressModel
    {
        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "2nd Address Line (optional)")]
        public string StreetAddress2 { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]        
        public string State { get; set; }

        [Required]
        public string Zip { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Phone { get; set; }
        
        [HiddenInput]
        public AddressType TypeOfAddress { get; set; }
        
        [ScaffoldColumn(false)]
        public string ClientScriptActionHint { get; set; }
    }

    public enum AddressType
    {
        Billing = 0,
        Shipping = 1
    }
}