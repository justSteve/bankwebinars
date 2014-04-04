using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class IncomingOrderModel
    {
        public IList<string> AdditionalLocations { get; set; }

        public string AdminComments { get; set; }
        public string AffiliateComments { get; set; }
        public string AlternativeEmail { get; set; }
        public Address BillingAddress { get; set; }
        public int Discount { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int idAffiliate { get; set; }
        public int idOption { get; set; }
        public int idWebinar { get; set; }
        public string Institution  { get; set; }
        public string LastName { get; set; }
        public string Origin { get; set; }
        public string Password { get; set; }
        public bool SendNotification { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderRowStatus Status { get; set; }
        public string Title { get; set; }
        public UserType UserType { get; set; }
        public USTimeZone UsTimeZone { get; set; }
        public string UserComments { get; set; }
    }
}