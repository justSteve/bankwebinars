 namespace CUWebinars.Web.Models

{
    public class JotFormWebHook
    {
        public string FormId { get; set; }
        public string SubmissionId { get; set; }
        //Create Type RegHook w/2 props .Name & .Body
        //register any possible endpoints
        // WebhookName = endpointUrl (currently is value of 'Webhook')
        // WebhookBody = Webhook + RawRequest
        public string Webhook { get; set; }
        public string FormTitle { get; set; }
        public string Pretty { get; set; }
        public string Username { get; set; }
        public string RawRequest { get; set; }
        public string Type { get; set; }
    }
}