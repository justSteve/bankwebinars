using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOrdersViewModel
    {
        public IEnumerable<OrderSummary> OrderSummaries { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }

    public class OrderSummary
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Institution { get; set; }
        public string BillingPhone { get; set; }
        public string BillingEmail { get; set; }
        public string BillingAddress { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }

    }
}