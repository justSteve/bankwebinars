using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class OnDemandPlaybackModel
    {
        public string AuthorizedToAccessMaterials { get; set; }
        public Presenter Presenter { get; set; }
        public Webinar Webinar { get; set; }
        public int idOrder { get; set; }
        public String OnDemandCode { get; set; }
        public IEnumerable<string> WebinarFiles { get; set; }
    }
}