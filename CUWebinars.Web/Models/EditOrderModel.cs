using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models
{
    public class EditOrderModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public decimal CostPerAdditionalLocation { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }

        public DateTime? PostEventAccessExpires { get; set; }
        public TagBuilder AdditionalLocationsRenderer { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public int Id { get; set; }
        public Discount Discount{ get; set; }
        public Order Order { get; set; }
        public RegType RegType { get; set; }
        public WebUser WebUser { get; set; }
        public ClaimsViewModel ClaimsViewModel { get; set; }
        
    }
}