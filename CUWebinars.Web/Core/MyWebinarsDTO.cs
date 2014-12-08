using System.Collections.Generic;
using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core
{
    public class MyWebinarsDTO
    {
        public OrderHasAdditionalLocationsViewModel OrderHasAdditionalLocationsViewModel { get; set; }
        public IList<Order> Scheduled { get; set; }
        public IList<Order> Recorded { get; set; }
        public IList<Order> Archived { get; set; }

        public WebUser WebUser { get; set; }
    }
}