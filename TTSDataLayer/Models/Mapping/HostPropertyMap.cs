using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class HostPropertyMap : EntityTypeConfiguration<HostProperty>
    {
        public HostPropertyMap()
        {
            // Primary Key
            this.HasKey(t => t.idHostProperty);

            // Properties
            this.Property(t => t.name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HostProperty");
            this.Property(t => t.idHostProperty).HasColumnName("idHostProperty");
            //this.Property(t => t.idHost).HasColumnName("idHost");
            this.Property(t => t.name).HasColumnName("name");

            // Relationships
            //this.HasRequired(t => t.Host)
            //    .WithMany(t => t.HostProperties)
            //    .HasForeignKey(d => d.idHost);

        }
    }
}
