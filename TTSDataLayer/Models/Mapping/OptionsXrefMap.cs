using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionsXrefMap : EntityTypeConfiguration<OptionsXref>
    {
        public OptionsXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.idOptionsXref);

            // Properties
            // Table & Column Mappings
            this.ToTable("OptionsXref");
            this.Property(t => t.idOptionsXref).HasColumnName("idOptionsXref");
            this.Property(t => t.idOptionGroup).HasColumnName("idOptionGroup");
            this.Property(t => t.idOption).HasColumnName("idOption");

            // Relationships
            this.HasRequired(t => t.Option)
                .WithMany(t => t.OptionsXrefs)
                .HasForeignKey(d => d.idOption);
            this.HasRequired(t => t.OptionsGroup)
                .WithMany(t => t.OptionsXrefs)
                .HasForeignKey(d => d.idOptionGroup);

        }
    }
}
