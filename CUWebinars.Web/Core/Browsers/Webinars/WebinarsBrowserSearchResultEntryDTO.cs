using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core.Browsers.Webinars
{
    public class WebinarsBrowserSearchResultEntryDTO
    {
        public Webinar Webinar { get; set; }
        public int RegistrationsCount { get; set; }
    }
}