using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AddAdditionalLocationsViewModel
    {
        public IList<string> Emails { get; set; }
        public AdditionalLocations AdditionalLocations { get; set; }
    }
}