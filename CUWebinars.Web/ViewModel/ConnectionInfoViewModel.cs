using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ConnectionInfoViewModel
    {
        public Webinar Webinar { get; set; }
        public IList<OrderRow> Orders { get; set; }
        public string ConnectionInfo { get; set; }
    }
}