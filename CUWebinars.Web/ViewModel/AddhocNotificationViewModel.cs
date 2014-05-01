using System.Web.Mvc;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdhocNotificationViewModel
    {
        public IEnumerable<SelectListItem> RegTypes { get; set; }
        public int SelectedUpcomingWebinarId { get; set; }
        public IEnumerable<SelectListItem> Webinars { get; set; }

    }
}