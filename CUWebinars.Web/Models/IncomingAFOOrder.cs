using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class IncomingAFOOrder
    {
        public string AffiliateComments { get; set; }

        public Address BillingAddress { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public int idAffiliate { get; set; }
        public int idRegType { get; set; }

        public string RegTypeLabel { get; set; }
        public int idWebinar { get; set; }
        public string Institution { get; set; }
        public string LastName { get; set; }
        public bool SendNotification { get; set; }
        public Address ShippingAddress { get; set; }

        public string Title { get; set; }

        public int rowNumber { get; set; }

        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public string SubmissionDate { get; set; }

        public string IP { get; set; }

        public string Origin { get; set; }
    }
}