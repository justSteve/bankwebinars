using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class HostPropertyValueMap : EntityTypeConfiguration<HostPropertyValue>
    {
        public HostPropertyValueMap()
        {
            // Primary Key
            this.HasKey(t => t.idHostPropertyValue);

            // Properties
            this.Property(t => t.value)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HostPropertyValue");
            this.Property(t => t.idHostPropertyValue).HasColumnName("idHostPropertyValue");
            this.Property(t => t.idHostProperty).HasColumnName("idHostProperty");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.value).HasColumnName("value");

            // Relationships
            this.HasRequired(t => t.HostProperty)
                .WithMany(t => t.HostPropertyValues)
                .HasForeignKey(d => d.idHostProperty);
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.HostPropertyValues)
                .HasForeignKey(d => d.idWebinar);

        }
    }
}
