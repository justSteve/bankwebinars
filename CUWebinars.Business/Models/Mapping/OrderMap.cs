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
            this.Property(t => t.FirstName)
                .HasMaxLength(100);

            this.Property(t => t.LastName)
                .HasMaxLength(100);

            this.Property(t => t.CustomerInstitution)
                .HasMaxLength(250);

            this.Property(t => t.Phone)
                .HasMaxLength(30);

            this.Property(t => t.Email)
                .HasMaxLength(150);

            this.Property(t => t.Address)
                .HasMaxLength(100);

            this.Property(t => t.City)
                .HasMaxLength(100);

            this.Property(t => t.State)
                .HasMaxLength(100);

            this.Property(t => t.Zip)
                .HasMaxLength(20);

            this.Property(t => t.ShippingFirstName)
                .HasMaxLength(100);

            this.Property(t => t.ShippingLastName)
                .HasMaxLength(100);

            this.Property(t => t.ShippingAddress)
                .HasMaxLength(100);

            this.Property(t => t.ShippingCity)
                .HasMaxLength(100);

            this.Property(t => t.ShippingState)
                .HasMaxLength(100);

            this.Property(t => t.ShippingZip)
                .HasMaxLength(20);

            this.Property(t => t.GeneralComments)
                .HasMaxLength(2550);

            this.Property(t => t.ShippingPhone)
                .HasMaxLength(30);

            this.Property(t => t.PaidByCCNumber)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("Orders");
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.idAffiliate).HasColumnName("idAffiliate");
            this.Property(t => t.OrderDate).HasColumnName("OrderDate");
            this.Property(t => t.Total).HasColumnName("Total");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.CustomerInstitution).HasColumnName("CustomerInstitution");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.Zip).HasColumnName("Zip");
            this.Property(t => t.ShippingFirstName).HasColumnName("ShippingFirstName");
            this.Property(t => t.ShippingLastName).HasColumnName("ShippingLastName");
            this.Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            this.Property(t => t.ShippingCity).HasColumnName("ShippingCity");
            this.Property(t => t.ShippingState).HasColumnName("ShippingState");
            this.Property(t => t.ShippingZip).HasColumnName("ShippingZip");
            this.Property(t => t.PaymentType).HasColumnName("PaymentType");
            this.Property(t => t.GeneralComments).HasColumnName("GeneralComments");
            this.Property(t => t.AuditInfo).HasColumnName("AuditInfo");
            this.Property(t => t.StoreComments).HasColumnName("StoreComments");
            this.Property(t => t.StoreCommentsPriv).HasColumnName("StoreCommentsPriv");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.InitiatedBy).HasColumnName("InitiatedBy");
            this.Property(t => t.ShippingPhone).HasColumnName("ShippingPhone");
            this.Property(t => t.PaidByCCNumber).HasColumnName("PaidByCCNumber");
            this.Property(t => t.Origin).HasColumnName("Origin");

            // Relationships
            this.HasRequired(t => t.Affiliate)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.idAffiliate);
            this.HasRequired(t => t.WebUser)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.idUser);

        }
    }
}
