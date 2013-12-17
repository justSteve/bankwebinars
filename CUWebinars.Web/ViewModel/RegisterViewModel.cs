using CUWebinars.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class RegisterViewModel
    {
        [UIHint("Register")]
        public RegisterModel RegisterFields { get; set; }
    }
}