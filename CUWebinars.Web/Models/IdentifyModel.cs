using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.Models
{
    public class IdentifyModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int idOrder { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public SignInModel SignInModel { get; set; }
    }
}