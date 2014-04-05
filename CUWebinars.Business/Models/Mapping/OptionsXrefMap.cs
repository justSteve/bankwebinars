using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionsXrefMap : EntityTypeConfiguration<OptionsXref>
    {
        public OptionsXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.idRegTypesXref);

            // Properties
            // Table & Column Mappings
            this.ToTable("OptionsXref");
            this.Property(t => t.idRegTypesXref).HasColumnName("idRegTypesXref");
            this.Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");
            this.Property(t => t.idRegType).HasColumnName("idRegType");

            // Relationships
            this.HasRequired(t => t.RegType)
                .WithMany(t => t.OptionsXrefs)
                .HasForeignKey(d => d.idRegType);
            this.HasRequired(t => t.OptionsGroup)
                .WithMany(t => t.OptionsXrefs)
                .HasForeignKey(d => d.idRegTypeGroup);

        }
    }
}
