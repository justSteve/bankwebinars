using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.Models
{
    public class BatchPasswordResetViewModel
    {
        [Required]
        [DisplayName("Users to Reset")]
        public string UserEmails { get; set; }
    }
}