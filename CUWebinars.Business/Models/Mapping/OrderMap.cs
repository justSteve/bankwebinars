
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            HasKey(t => t.idOrder);

            // Properties
            Property(t => t.FirstName).HasMaxLength(100);
            Property(t => t.LastName).HasMaxLength(100);
            Property(t => t.Institution).HasMaxLength(250);
            Property(t => t.BillingPhone).HasMaxLength(30);
            Property(t => t.BillingEmail).HasMaxLength(150);
            Property(t => t.BillingAddress).HasMaxLength(200);
            Property(t => t.BillingAddress2).HasMaxLength(200);
            Property(t => t.BillingCity).HasMaxLength(100);
            Property(t => t.BillingState).HasMaxLength(100);
            Property(t => t.BillingZip).HasMaxLength(20);
            Property(t => t.ShippingFirstName).HasMaxLength(100);
            Property(t => t.ShippingLastName).HasMaxLength(100);
            Property(t => t.ShippingAddress).HasMaxLength(100);
            Property(t => t.ShippingAddress2).HasMaxLength(100);
            Property(t => t.ShippingCity).HasMaxLength(100);
            Property(t => t.ShippingState).HasMaxLength(100);
            Property(t => t.ShippingZip).HasMaxLength(20);
            Property(t => t.UserComments).HasMaxLength(2550);
            Property(t => t.ShippingPhone).HasMaxLength(30);

            // Table & Column Mappings
            ToTable("Order");
            Property(t => t.idOrder).HasColumnName("idOrder");
            Property(t => t.idUser).HasColumnName("idUser");
            Property(t => t.idAffiliate).HasColumnName("idAffiliate");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.Total).HasColumnName("Total");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Institution).HasColumnName("Institution");
            Property(t => t.BillingPhone).HasColumnName("BillingPhone");
            Property(t => t.BillingEmail).HasColumnName("BillingEmail");
            Property(t => t.BillingAddress).HasColumnName("BillingAddress");
            Property(t => t.BillingAddress2).HasColumnName("BillingAddress2");
            Property(t => t.BillingCity).HasColumnName("BillingCity");
            Property(t => t.BillingState).HasColumnName("BillingState");
            Property(t => t.BillingZip).HasColumnName("BillingZip");
            Property(t => t.ShippingFirstName).HasColumnName("ShippingFirstName");
            Property(t => t.ShippingLastName).HasColumnName("ShippingLastName");
            Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            Property(t => t.ShippingAddress2).HasColumnName("ShippingAddress2");
            Property(t => t.ShippingCity).HasColumnName("ShippingCity");
            Property(t => t.ShippingState).HasColumnName("ShippingState");
            Property(t => t.ShippingZip).HasColumnName("ShippingZip");
            Property(t => t.PaymentType).HasColumnName("PaymentType");
            Property(t => t.UserComments).HasColumnName("UserComments");
            Property(t => t.AuditInfo).HasColumnName("AuditInfo");
            Property(t => t.AffiliateComments).HasColumnName("AffiliateComments");
            Property(t => t.AdminComments).HasColumnName("AdminComments");
            Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            Property(t => t.ShippingPhone).HasColumnName("ShippingPhone");
            Property(t => t.Origin).HasColumnName("Origin");

            Ignore(t => t.DomainEntityState);
            Ignore(t => t.OriginalValues);


            // Relationships
            HasRequired(t => t.Affiliate).WithMany(t => t.Orders).HasForeignKey(d => d.idAffiliate);
            HasRequired(t => t.WebUser).WithMany(t => t.Orders).HasForeignKey(d => d.idUser);

        }
    }
}
