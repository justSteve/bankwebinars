using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Order
    {
        public Order()
        {
            this.OrderRows = new List<OrderRow>();
        }

        public int idOrder { get; set; }
        public int idUser { get; set; }
        public int idAffiliate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
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
        public byte PaymentType { get; set; }
        public string UserComments { get; set; }
        public string AuditInfo { get; set; }
        public string AffiliateComments { get; set; }
        public string AdminComments { get; set; }
        public bool TaxExempt { get; set; }
        public byte InitiatedBy { get; set; }
        public string PaidByCCNumber { get; set; }
        public string Origin { get; set; }
        public virtual Affiliate Affiliate { get; set; }
        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual WebUser WebUser { get; set; }
    }
}
