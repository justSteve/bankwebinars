using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ClickToJoinViewModel
    {
        public string JoinCode { get; set; }
        public string RedirectLinkText { get; set; }
        public Webinar Webinar { get; set; }
    }
}