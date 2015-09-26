using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class GenerateClickToJoinViewModel
    {
        public string Email { get; set; }

        public int SelectedWebinarId { get; set; }

        public int? OrderId { get; set; }
        public IEnumerable<SelectListItem> Webinars { get; set; }
    }
}