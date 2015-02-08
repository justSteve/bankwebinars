using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class WebinarPromoViewModel
    {
        public ClaimsIdentity Identity { get; set; }
        public string TemplateType { get; set; } // will the template be the 'per day' or  per week
        public IEnumerable<WebinarFile> WebinarFiles { get; set; }
        
        public string TimeFormatDisplay { get; set; }
        public USTimeZone TimeZone { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
        
        public Webinar Webinar { get; set; }
        public Affiliate Affiliate { get; set; }
    }
}
