using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    
    public class ChangeEmailFromKeyInputModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [HiddenInput]
        public string Key { get; set; }
    }
}