using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class CcLocationsViewModel
    {
        public ICollection<Business.Models.CcLocation> CcLocations { get; set; }
        public string Addresses { get; set; }
        public TagBuilder CcLocationsRenderer { get; set; }
    }
}