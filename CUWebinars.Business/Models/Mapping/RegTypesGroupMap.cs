using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypesGroupMap : EntityTypeConfiguration<RegTypesGroup>
    {
        public RegTypesGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.idRegTypeGroup);

            // Properties
            this.Property(t => t.RegTypeGroupDesc)
                .HasMaxLength(50);

            this.Property(t => t.RegTypeType)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("RegTypesGroups");
            this.Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");
            this.Property(t => t.RegTypeGroupDesc).HasColumnName("RegTypeGroupDesc");
            this.Property(t => t.RegTypeType).HasColumnName("RegTypeType");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
