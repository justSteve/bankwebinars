using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditPresenterModel
    {
        public string PhotoFull { get; set; }
        public string PhotoThumb { get; set; }
        public string BioLong { get; set; }
        public string BioShort { get; set; }
        public WebUser WebUser { get; set; }

        public int? idWebUser { get; set; }

        [Display(Name = "Sage Account")]
        public int? SageAccountId { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }
        
    }
}