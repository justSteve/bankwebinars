using CUWebinars.Business.Models;

namespace CUWebinars.Web.Controllers
{
    public class CertOfCompletionDSViewModel
    {
        public Webinar Webinar { get; set; }
        public string DisplayName { get; set; }
        public string CeuShort { get; set; }
        public string CeuStatement { get; set; }
        public string DisplayInst { get; set; }
    }
}