using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class ChangePasswordFromResetKeyInputModel
    {
        [ScaffoldColumn(false)]
        public bool ChangePasswordSucceeded { get; set; }

        [Required]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password confirmation must match password.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [HiddenInput]
        public string Key { get; set; }
    }
}