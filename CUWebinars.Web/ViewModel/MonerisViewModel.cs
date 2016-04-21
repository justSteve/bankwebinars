using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class MonerisViewModel
    {

        public PricesAndDiscounts PricesAndDiscounts { get; set; }
        public decimal TaxAmount { get; set; }
        public string Item { get; set; }
        public int idOrder { get; set; }
        public string BillingEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Institution { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingZip { get; set; }
        public string BillingPhone { get; set; }
        public string ShippingEmail { get; set; }
        public string FirstNameShipping { get; set; }
        public string LastNameShipping { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public string ShippingPhone { get; set; }
    }
}