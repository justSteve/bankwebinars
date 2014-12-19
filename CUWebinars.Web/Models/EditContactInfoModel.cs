using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditContactInfoModel
    {
        public bool HasLocalPassword { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("Register")]
        public RegisterModel RegisterFields { get; set; }
        
            }
}