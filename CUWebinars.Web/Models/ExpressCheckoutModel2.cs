namespace CUWebinars.Web.Models
{
    public class ExpressCheckoutModel2
    {
        public string Slug { get; set; }
        public string WebinarTitle { get; set; }
        public string RegistrationType { get; set; }
        public FullName FullName { get; set; }
        public string Title { get; set; }
        public string Institution { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public Address4JotForm Address { get; set; }
        public int WebinarId { get; set; }
        public int OrderId { get; set; }
        public int AffiliateId { get; set; }
        public string EventId { get; set; }
    }
}