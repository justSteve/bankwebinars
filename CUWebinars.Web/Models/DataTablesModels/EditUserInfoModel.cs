using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CUWebinars.Web.Models.DataTablesModels
{
    public class EditUserInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturnUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditUser")]
        public EditUserModel EditFields { get; set; }
    }
}