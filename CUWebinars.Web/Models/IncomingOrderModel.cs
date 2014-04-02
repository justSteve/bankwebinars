using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class IncomingOrderModel
    {
        public string AdminComments { get; set; }
        public string AffiliateComments { get; set; }
        public string AlternativeEmail { get; set; }
        public Address BillingAddress { get; set; }
        public int Discount { get; set; }
        public string Email { get; set; }
        public int idAffiliate { get; set; }
        public int idOption { get; set; }
        public int idOrder { get; set; }
        public int idWebinar { get; set; }
        public IList<OrderRowOption> OrderRowOptions { get; set; }
        public string Origin { get; set; }
        public int RegistrationType { get; set; }
        public Address ShippingAddress { get; set; }
        public int Status { get; set; }
        public string UserComments { get; set; }
    }
}