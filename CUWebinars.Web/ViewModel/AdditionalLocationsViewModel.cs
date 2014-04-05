using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationsViewModel
    {
        public AddAdditionalLocationsViewModel AddAdditionalLocationsViewModel { get; set; }
        public AdditionalLocations AdditionalLocations { get; set; }
        public IList<AdditionalEmails> Locations { get; set; }
        public Webinar Webinar { get; set; }
    }
}