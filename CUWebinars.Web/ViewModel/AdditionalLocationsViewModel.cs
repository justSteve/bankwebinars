using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationsViewModel
    {
        public AddAdditionalLocationsViewModel AddAdditionalLocationsViewModel { get; set; }
        public OrderRowOption OrderRowOption { get; set; }
        public IList<AdditionalLocation> Locations { get; set; }
        public Webinar Webinar { get; set; }
    }
}