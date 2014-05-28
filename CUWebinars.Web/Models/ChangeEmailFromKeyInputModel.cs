using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.Models
{

    public class ChangeEmailFromKeyInputModel : LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string ScreenMessage { get; set; }
    }
}