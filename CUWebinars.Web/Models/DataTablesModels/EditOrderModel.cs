using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Models.DataTablesModels
{
    public class EditOrderModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public decimal CostPerAdditionalLocation { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }
        public bool AdditionalLocationsAvailableOnLoad { get; set; }
        
        public DateTime? PostEventAccessExpires { get; set; }
        public TagBuilder AdditionalLocationsRenderer { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public int idOrderRow { get; set; }
        public DiscountModel Discount{ get; set; }
        public Order Order { get; set; }
        public RegType RegType { get; set; }
        public ICollection<RegType> RegTypes { get; set; }

        public int UserId { get; set; }
        public int WebinarId { get; set; }
        public WebUser WebUser { get; set; }
        public ClaimsViewModel ClaimsViewModel { get; set; }
        public PostEventClaim PostEventClaim { get; set; }
        public String AuditInfo { get; set; }
    }
}