using System;
using CUWebinars.Business.Models;
using CUWebinars.Web.ViewModel;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class ManageOrderEditModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public DisplayOptionsInDropDownViewModel DisplayOptionsInDropDownViewModel { get; set; }
        public decimal CostPerAdditionalLocation { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }

        public DateTime? PostEventAccessExpires { get; set; }
        public TagBuilder AdditionalLocationsRenderer { get; set; }
        public int NumberOfAdditionalLocations { get; set; }
        public int Id { get; set; }
        public Order Order { get; set; }
        public int RegType { get; set; }
        public bool AdditionalLocationsAvailableOnLoad { get; set; }
        public string JoinCode { get; set; }
        public string PhoneNumber { get; set; }
        public string AffiliateName { get; set; }
        public int UserId { get; set; }
        public int WebinarId { get; set; }
        public WebUser WebUser { get; set; }
    }
}