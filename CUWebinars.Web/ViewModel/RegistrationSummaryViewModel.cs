using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class RegistrationSummaryViewModel
    {
        public AdditionalLocationsViewModel AdditionalLocationsViewModel { get; set; }
        public OrderRow OrderRow { get; set; }
        public string RecordingLink { get; set; }
        public int DisplayPostEventMaterials { get; set; }
        public IEnumerable<string> WebinarFiles { get; set; }
        public WebinarStatus WebinarStatus { get; set; }
    }
}