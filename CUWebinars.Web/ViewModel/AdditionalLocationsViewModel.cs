using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AdditionalLocationsViewModel
    {
        public string[] Emails { get; set; }
        public OrderRowOption AdditionalLocation { get; set; }
        public Webinar Webinar { get; set; }
    }
}