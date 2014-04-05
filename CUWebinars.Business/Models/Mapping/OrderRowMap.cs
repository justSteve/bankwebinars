using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowMap : EntityTypeConfiguration<OrderRow>
    {
        public OrderRowMap()
        {
            // Primary Key
            this.HasKey(t => t.idOrderRow);

            // Table & Column Mappings
            this.ToTable("OrderRow");
            this.Property(t => t.idOrderRow).HasColumnName("idOrderRow");
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.RowPrice).HasColumnName("RowPrice");
            //this.Property(t => t.RowStatus).HasColumnName("RowStatus");
            this.Property(t => t.ShipmentDate).HasColumnName("ShipmentDate");
            this.Property(t => t.AccessExpires).HasColumnName("AccessExpires");
            this.Property(t => t.Royalty).HasColumnName("Royalty");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany(t => t.OrderRows)
                .HasForeignKey(d => d.idOrder);
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.OrderRows)
                .HasForeignKey(d => d.idWebinar);
            this.HasOptional(t => t.AdditionalLocation)
                .WithOptionalDependent(t => t.OrderRow);
        }
    }
}
