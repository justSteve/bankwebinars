using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditNameTitleModel
    {
        [Required]
        [Display(Name = "First")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last")]
        public string LastName { get; set; }

        public string Title { get; set; }
        public int? idWebUser { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }

    }
}