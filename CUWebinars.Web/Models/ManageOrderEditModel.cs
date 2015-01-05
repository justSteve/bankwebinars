using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class ManageOrderEditModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public DisplayOptionsInDropDownViewModel DisplayOptionsInDropDownViewModel { get; set; }
        public decimal CostPerAdditionalLocation { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }

        public TagBuilder AdditionalLocationsRenderer { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public int Id { get; set; }
        public int RegType { get; set; }
        public int UserId { get; set; }
        public int WebinarId { get; set; }
    }
}