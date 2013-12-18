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
        public System.DateTime OrderDate { get; set; }
        public Nullable<decimal> Total { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerInstitution { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZip { get; set; }
        public byte PaymentType { get; set; }
        public string GeneralComments { get; set; }
        public string AuditInfo { get; set; }
        public string StoreComments { get; set; }
        public string StoreCommentsPriv { get; set; }
        public bool TaxExempt { get; set; }
        public byte InitiatedBy { get; set; }
        public string ShippingPhone { get; set; }
        public string PaidByCCNumber { get; set; }
        public string Origin { get; set; }
        public virtual Affiliate Affiliate { get; set; }
        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual WebUser WebUser { get; set; }
    }
}
