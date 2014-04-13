
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowMap : EntityTypeConfiguration<OrderRow>
    {
        public OrderRowMap()
        {
            // Primary Key
            HasKey(t => t.idOrderRow);

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

            
            


            // Relationships
            HasRequired(t => t.Order)
                            .WithMany(t => t.OrderRows)
                            .HasForeignKey(d => d.idOrder);
            HasRequired(t => t.Webinar)
                            .WithMany(t => t.OrderRows)
                            .HasForeignKey(d => d.idWebinar);
            //error after switching to 'ICollection' in orderRow.
            //HasOptional(t => t.AdditionalLocation)
            //                .WithOptionalDependent(t => t.OrderRow);
            
        }
    }
}
