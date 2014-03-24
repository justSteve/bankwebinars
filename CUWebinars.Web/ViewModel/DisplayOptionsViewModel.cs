using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOptionsViewModel
    {
        public AdditionalLocationsViewModel AdditionalLocationsViewModel { get; set; }
        public List<Option> Options { get; set; }
        public Webinar Webinar { get; set; }
    }
}