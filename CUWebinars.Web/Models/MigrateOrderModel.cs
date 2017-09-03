using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class MigrateOrderModel
    {
        public string AdditionalLocationsString { get; set; }

        public string AdminComments { get; set; }
        public Address BillingAddress { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int  idAffiliate { get; set; }
        public int idRegType { get; set; }
        public int LegacyRegType { get; set; }
        public int idWebinar { get; set; }
        public int idOrderLegacy { get; set; }
        //public int idUserLegacy { get; set; }
        public string Institution  { get; set; }
        public string Origin  { get; set; }
        public string Title { get; set; }
        public string LastName { get; set; }
        public bool SendNotification { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public string DiscountCode{ get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShipmentDate { get; set; }
        public decimal Total { get; set; }
        public string LoggerNotes { get; set; }
    }

}