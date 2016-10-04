using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.ViewModel
{
    public class RegistrationSummaryMultiViewModel
    {
        public List<RegistrationSummaryViewModel> RegistrationSummaryViewModels { get; set; }
        public string DiscountCaptionMulti { get; set; }
        public string GrandTotalCaptionMulti { get; set; }
    }
}