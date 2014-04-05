using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionsGroupsXrefMap : EntityTypeConfiguration<OptionsGroupsXref>
    {
        public OptionsGroupsXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.idWebinarOptionGroup);

            // Properties
            // Table & Column Mappings
            this.ToTable("OptionsGroupsXref");
            this.Property(t => t.idWebinarOptionGroup).HasColumnName("idWebinarOptionGroup");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");

            // Relationships
            this.HasRequired(t => t.OptionsGroup)
                .WithMany(t => t.OptionsGroupsXrefs)
                .HasForeignKey(d => d.idRegTypeGroup);
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.OptionsGroupsXrefs)
                .HasForeignKey(d => d.idWebinar);

        }
    }
}
