using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            this.HasKey(t => t.idOrder);

            // Properties
            this.Property(t => t.FirstName).HasMaxLength(100);
            this.Property(t => t.LastName).HasMaxLength(100);
            this.Property(t => t.Institution).HasMaxLength(250);
            this.Property(t => t.BillingPhone).HasMaxLength(30);
            this.Property(t => t.BillingEmail).HasMaxLength(150);
            this.Property(t => t.BillingAddress).HasMaxLength(200);
            this.Property(t => t.BillingAddress2).HasMaxLength(200);
            this.Property(t => t.BillingCity).HasMaxLength(100);
            this.Property(t => t.BillingState).HasMaxLength(100);
            this.Property(t => t.BillingZip).HasMaxLength(20);
            this.Property(t => t.ShippingFirstName).HasMaxLength(100);
            this.Property(t => t.ShippingLastName).HasMaxLength(100);
            this.Property(t => t.ShippingAddress).HasMaxLength(100);
            this.Property(t => t.ShippingAddress2).HasMaxLength(100);
            this.Property(t => t.ShippingCity).HasMaxLength(100);
            this.Property(t => t.ShippingState).HasMaxLength(100);
            this.Property(t => t.ShippingZip).HasMaxLength(20);
            this.Property(t => t.UserComments).HasMaxLength(2550);
            this.Property(t => t.ShippingPhone).HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Order");
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.idAffiliate).HasColumnName("idAffiliate");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.Institution).HasColumnName("Institution");
            this.Property(t => t.BillingPhone).HasColumnName("BillingPhone");
            this.Property(t => t.BillingEmail).HasColumnName("BillingEmail");
            this.Property(t => t.BillingAddress).HasColumnName("BillingAddress");
            this.Property(t => t.BillingAddress2).HasColumnName("BillingAddress2");
            this.Property(t => t.BillingCity).HasColumnName("BillingCity");
            this.Property(t => t.BillingState).HasColumnName("BillingState");
            this.Property(t => t.BillingZip).HasColumnName("BillingZip");
            this.Property(t => t.ShippingFirstName).HasColumnName("ShippingFirstName");
            this.Property(t => t.ShippingLastName).HasColumnName("ShippingLastName");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.ShippingAddress2).HasColumnName("ShippingAddress2");
            this.Property(t => t.ShippingCity).HasColumnName("ShippingCity");
            this.Property(t => t.ShippingState).HasColumnName("ShippingState");
            this.Property(t => t.ShippingZip).HasColumnName("ShippingZip");
            this.Property(t => t.PaymentType).HasColumnName("PaymentType");
            this.Property(t => t.UserComments).HasColumnName("UserComments");
            this.Property(t => t.AuditInfo).HasColumnName("AuditInfo");
            this.Property(t => t.AffiliateComments).HasColumnName("AffiliateComments");
            this.Property(t => t.AdminComments).HasColumnName("AdminComments");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.ShippingPhone).HasColumnName("ShippingPhone");
            this.Property(t => t.Origin).HasColumnName("Origin");

            // Relationships
            this.HasRequired(t => t.Affiliate).WithMany(t => t.Orders).HasForeignKey(d => d.idAffiliate);
            this.HasRequired(t => t.WebUser).WithMany(t => t.Orders).HasForeignKey(d => d.idUser);

        }
    }
}
