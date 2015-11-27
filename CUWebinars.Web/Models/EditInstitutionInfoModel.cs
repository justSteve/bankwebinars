using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CUWebinars.Web.Models
{
    public class EditInstitutionInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturlUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditInstitution")]
        public EditInstitutionModel EditFields { get; set; }
        
    }
}