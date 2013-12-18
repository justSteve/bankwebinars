using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OptionsGroupMap : EntityTypeConfiguration<OptionsGroup>
    {
        public OptionsGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.idOptionGroup);

            // Properties
            this.Property(t => t.OptionGroupDesc)
                .HasMaxLength(50);

            this.Property(t => t.OptionType)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("OptionsGroups");
            this.Property(t => t.idOptionGroup).HasColumnName("idOptionGroup");
            this.Property(t => t.OptionGroupDesc).HasColumnName("OptionGroupDesc");
            this.Property(t => t.OptionType).HasColumnName("OptionType");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
