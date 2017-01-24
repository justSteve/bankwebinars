using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CUWebinars.Business.Models;
using CUWebinars.Web.Helpers;

namespace CUWebinars.Web.Models
{
    public class EditPresenterInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public ClaimsIdentity LoggedInUser { get; set; }
        public string ReturnUrl { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditPresenter")]
        public EditPresenterModel EditFields { get; set; }

        public WebUser WebUser { get; set; }
    }
}