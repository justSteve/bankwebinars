using System.Web.Mvc;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdhocNotificationViewModel
    {
        public IEnumerable<SelectListItem> RegTypes { get; set; }
        public int SelectedOrderId { get; set; }
        public int SelectedWebinarId { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<SelectListItem> OrdersList { get; set; }
        public IEnumerable<SelectListItem> Webinars { get; set; }

    }
}