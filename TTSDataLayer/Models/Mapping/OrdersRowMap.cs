using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowMap : EntityTypeConfiguration<OrderRow>
    {
        public OrderRowMap()
        {
            // Primary Key
            this.HasKey(t => t.idOrderRow);

            // Properties
            this.Property(t => t.AlternateEmail)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("OrderRow");
            this.Property(t => t.idOrderRow).HasColumnName("idOrderRow");
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            this.Property(t => t.RowPrice).HasColumnName("RowPrice");
            this.Property(t => t.idDiscount).HasColumnName("idDiscount");
            this.Property(t => t.DiscountPercentOff).HasColumnName("DiscountPercentOff");
            this.Property(t => t.DiscountFlatOff).HasColumnName("DiscountFlatOff");
            this.Property(t => t.AlternateEmail).HasColumnName("AlternateEmail");
            this.Property(t => t.RegistrationType).HasColumnName("RegistrationType");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.ShipmentDate).HasColumnName("ShipmentDate");
            this.Property(t => t.isAdHocRecording).HasColumnName("isAdHocRecording");
            this.Property(t => t.Royalty).HasColumnName("Royalty");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany(t => t.OrderRow)
                .HasForeignKey(d => d.idOrder);
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.OrderRow)
                .HasForeignKey(d => d.idWebinar);

        }
    }
}
