using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditInstitutionModel
    {

        [Required]
        public string Institution { get; set; }

        public int? idWebUser { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }

    }
}