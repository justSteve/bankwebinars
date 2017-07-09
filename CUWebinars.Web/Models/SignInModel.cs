using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class SignInModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
        
        [Display(Name="Remember Me")]
        public bool RememberMe { get; set; }
        
        [ScaffoldColumn(false)]
        public bool SigninAfterCheckout { get; set; }
    }
}