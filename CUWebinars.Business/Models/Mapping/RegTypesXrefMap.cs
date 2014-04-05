using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypesXrefMap : EntityTypeConfiguration<RegTypesXref>
    {
        public RegTypesXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.idRegTypesXref);

            // Properties
            // Table & Column Mappings
            this.ToTable("RegTypesXref");
            this.Property(t => t.idRegTypesXref).HasColumnName("idRegTypesXref");
            this.Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");
            this.Property(t => t.idRegType).HasColumnName("idRegType");

            // Relationships
            //this.HasRequired(t => t.RegType)
            //    .WithMany(t => t.RegTypeXrefs)
            //    .HasForeignKey(d => d.idRegType);
            this.HasRequired(t => t.RegTypesGroup)
                .WithMany(t => t.RegTypesXrefs)
                .HasForeignKey(d => d.idRegTypeGroup);

        }
    }
}
