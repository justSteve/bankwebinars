
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypesGroupMap : EntityTypeConfiguration<RegTypesGroup>
    {
        public RegTypesGroupMap()
        {
            // Primary Key
HasKey(t => t.idRegTypeGroup);

            // Properties
Property(t => t.RegTypeGroupDesc)
                .HasMaxLength(50);

Property(t => t.RegTypeType)
                .HasMaxLength(1);

            // Table & Column Mappings
ToTable("RegTypesGroups");
Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");
Property(t => t.RegTypeGroupDesc).HasColumnName("RegTypeGroupDesc");
Property(t => t.RegTypeType).HasColumnName("RegTypeType");
Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
