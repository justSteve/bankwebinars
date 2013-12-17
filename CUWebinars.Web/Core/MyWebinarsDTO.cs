using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core
{
    public class MyWebinarsDTO
    {
        public IDictionary<Option, Order> Scheduled { get; set; }
        public IList<Order> Recorded { get; set; }
        public IList<Order> Archived { get; set; }

        public WebUser WebUser { get; set; }
    }
}