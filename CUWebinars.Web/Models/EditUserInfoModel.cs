using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditUserInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("EditUser")]
        public EditUserModel EditFields { get; set; }
    }
}