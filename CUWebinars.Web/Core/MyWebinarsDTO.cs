using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;
using System.Collections;
using System.Collections.Generic;

namespace CUWebinars.Web.Core
{
    public class MyWebinarsDTO
    {

        public AdditionalLocationsViewModel AdditionalLocationsViewModel { get; set; }
        public IList<RegistrationSummaryViewModel> Scheduled { get; set; }
        public IDictionary<string, RegistrationSummaryViewModel> Recorded { get; set; }
        public IList<RegistrationSummaryViewModel> Archived { get; set; }
        public IEnumerable MyClaims { get; set; }
        public DiscountModel Subscription { get; set; }
        public DiscountModel Package { get; set; }
        public CompliancePerspectivesModel CompliancePerspectives { get; set; }
         
        public WebUser WebUser { get; set; }
        public string PromptRefresh { get; set; }
    }
}