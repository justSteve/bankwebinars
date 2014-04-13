
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class RegTypesGroupsXrefMap : EntityTypeConfiguration<RegTypesGroupsXref>
    {
        public RegTypesGroupsXrefMap()
        {
            // Primary Key
            HasKey(t => t.idWebinarRegTypeGroup);

            // Properties
            // Table & Column Mappings
            ToTable("RegTypesGroupsXref");
            Property(t => t.idWebinarRegTypeGroup).HasColumnName("idWebinarRegTypeGroup");
            Property(t => t.idWebinar).HasColumnName("idWebinar");
            Property(t => t.idRegTypeGroup).HasColumnName("idRegTypeGroup");

            
            


            // Relationships
            HasRequired(t => t.RegTypesGroup)
                            .WithMany(t => t.RegTypesGroupsXrefs)
                            .HasForeignKey(d => d.idRegTypeGroup);
            //this.HasRequired(t => t.Webinar)
            //    .WithMany(t => t.RegTypesGroupsXrefs)
            //    .HasForeignKey(d => d.idWebinar);

        }
    }
}
