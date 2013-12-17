
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace CUWebinars.Web.Models
{
    public class ManageModel
    {
        public bool HasLocalPassword { get; set; }
        public string StatusMessage { get; set; }

        [UIHint("Register")]
        public RegisterModel RegisterFields { get; set; }
    }
}