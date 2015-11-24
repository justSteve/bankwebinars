using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturlUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditInstitution")]
        public EditInstitutionModel EditFields { get; set; }

    }
}