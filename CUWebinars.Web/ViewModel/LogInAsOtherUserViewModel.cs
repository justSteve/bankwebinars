using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.ViewModel
{
    public class LogInAsOtherUserViewModel : EmailRequiredBaseViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Enter your password")]
        public string Password { get; set; }

    }
}