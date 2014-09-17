using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AddAdditionalLocationViewModel
    {
        public List<AdditionalLocation> AdditionalLocation { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
    }
}