using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Commands
{
    public class MigrateOrderCommand
    {
        public Affiliate Affiliate { get; set; }
        public string AffiliateComments { get; set; }
        public Address BillingAddress { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string Email { get; set; }
        public string AdditionalLocationsString { get; set; }
        public decimal Total { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public OrderRow OrderRow { get; set; }
        public Address ShippingAddress { get; set; }
        
        public int idOrderLegacy { get; set; }
        //public int idUserLegacy { get; set; }
        public DateTime OrderDate { get; set; }

        public string VerificationKey { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }

        //  Out parameter
        public int OrderId { get; set; }

        public string Tenant { get; set; }
    }
}
