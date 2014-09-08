namespace CUWebinars.Web.Models
{
    public class JotFormModel
    {
        public string formID { get; set; }
        public string submissionID { get; set; }
        public string webhookURL { get; set; }
        public string formTitle { get; set; }
        public string pretty { get; set; }
        public string username { get; set; }
        public string rawRequest { get; set; }
        public string type { get; set; }
    }
}