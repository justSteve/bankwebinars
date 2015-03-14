using System.ComponentModel.DataAnnotations;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AdjustUserDetailsEditModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Institution { get; set; }

        public int idUser { get; set; }
    }
}