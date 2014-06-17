using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Commands
{
    public class RegisterNewAccountCommand 
    {
        public Address BillingAddress { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Institution { get; set; }
        public Address ShippingAddress { get; set; }
        public string TempPassword { get; set; }
        public string Tenant { get; set; }
        public string Title{ get; set; }

        //  out parameter
        public WebUser WebUser { get; set; }
    }
}
