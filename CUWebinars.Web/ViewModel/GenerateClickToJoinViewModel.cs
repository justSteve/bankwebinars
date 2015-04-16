using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.ViewModel
{
    public class GenerateClickToJoinViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int? WebinarId { get; set; }
    }
}