
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowMap : EntityTypeConfiguration<OrderRow>
    {
        public OrderRowMap()
        {
            // Primary Key
            HasKey(t => t.idOrderRow);
            
            Property(o => o.RegistrantKey).HasMaxLength(25);
            Property(o => o.JoinURL).HasMaxLength(125);
            Property(o => o.Royalty).HasPrecision(8,2);
            Property(o => o.UnitPrice).HasPrecision(8,2);
            Property(o => o.RowPrice).HasPrecision(8,2);
            Property(o => o.TtsJoinUrl).IsVariableLength().IsOptional().HasMaxLength(10);


            // Table & Column Mappings
            ToTable("OrderRow");
            Property(t => t.idOrderRow).HasColumnName("idOrderRow");
            Property(t => t.idOrder).HasColumnName("idOrder");
            Property(t => t.idWebinar).HasColumnName("idWebinar");
            Property(t => t.RegistrantKey).HasColumnName("RegistrantKey");
            Property(t => t.RowPrice).HasColumnName("RowPrice");
            Property(t => t.RowStatus).HasColumnName("RowStatus");
            Property(t => t.ShipmentDate).HasColumnName("ShipmentDate");
            Property(t => t.AccessExpires).HasColumnName("AccessExpires");
            Property(t => t.Royalty).HasColumnName("Royalty");
            Property(t => t.TtsJoinUrl).HasColumnName("TtsJoinUrl");

            // Relationships
            HasRequired(t => t.Order)
                            .WithMany(t => t.OrderRows)
                            .HasForeignKey(d => d.idOrder);
            HasRequired(t => t.Webinar)
                            .WithMany(t => t.OrderRows)
                            .HasForeignKey(d => d.idWebinar);
        }
    }
}
