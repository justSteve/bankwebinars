using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationAddViewModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public decimal Price { get; set; }
    }
}