using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationsViewModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public string Addresses { get; set; }
        public decimal OptionsCost { get; set; }
        public TagBuilder AdditionalLocationsRenderer { get; set; }
        public EditAdditionalLocationsViewModel EditAdditionalLocationsViewModel { get; set; }
    }
}