
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypesXrefMap : EntityTypeConfiguration<RegTypesXref>
    {
        public RegTypesXrefMap()
        {
            // Primary Key
            HasKey(t => t.idRegTypesXref);

            // Properties
            // Table & Column Mappings
            ToTable("RegTypesXref");
            Property(t => t.idRegTypesXref).HasColumnName("idRegTypesXref");
            Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");
            Property(t => t.idRegType).HasColumnName("idRegType");

            
            


            // Relationships
            //this.HasRequired(t => t.RegType)
            //    .WithMany(t => t.RegTypeXrefs)
            //    .HasForeignKey(d => d.idRegType);
            HasRequired(t => t.RegTypesGroup)
                            .WithMany(t => t.RegTypesXrefs)
                            .HasForeignKey(d => d.idRegTypeGroup);

        }
    }
}
