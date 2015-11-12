using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CertOfCompletionViewModel
    {
        public Order Order { get; set; }
        public string DisplayName { get; set; }
        public string DisplayInst { get; set; }
        public string CeuShort { get; set; }
        public string CeuStatement { get; set; }
    }
}