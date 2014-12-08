using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class OrderHasAdditionalLocationsViewModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public string Addresses { get; set; }
        public decimal OptionsCost { get; set; }
    }
}