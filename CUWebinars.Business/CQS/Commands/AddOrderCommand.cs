using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Commands
{
    public class AddOrderCommand
    {
        public Affiliate Affiliate { get; set; }
        public string AffiliateComments { get; set; }
        public string AdminComments { get; set; }
        public string UserComments { get; set; }
        public Address BillingAddress { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public OrderRow OrderRow { get; set; }
        public OrderGenesis OrderGenesis { get; set; }
        public Address ShippingAddress { get; set; }
        public string Tenant { get; set; }

        public string VerificationKey { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }

        //  Out parameter
        public int OrderId { get; set; }
    }
}
