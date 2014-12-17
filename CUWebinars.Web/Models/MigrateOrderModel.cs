using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class MigrateOrderModel
    {
        public IList<IncomingAdditionalLocation> AdditionalLocations { get; set; }

        public string AffiliateComments { get; set; }
        public Address BillingAddress { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int  idAffiliate { get; set; }
        public int idRegType { get; set; }
        public int idWebinar { get; set; }
        public int idOrderLegacy { get; set; }
        public string Institution  { get; set; }
        public string Origin  { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public bool SendNotification { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public int idDiscount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipmentDate { get; set; }
        public decimal Total { get; set; }


    }

}