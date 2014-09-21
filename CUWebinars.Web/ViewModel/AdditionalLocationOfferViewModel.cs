using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationOfferViewModel
    {
        public AdditionalLocation AdditionalLocation { get; set; }
        public Webinar Webinar { get; set; }
        public decimal Price { get; set; }
        public WebUser WebUser { get; set; }
    }
}