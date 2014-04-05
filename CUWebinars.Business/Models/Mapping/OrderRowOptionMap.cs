using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowRegTypeMap : EntityTypeConfiguration<AdditionalLocations>
    {
        public OrderRowRegTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.idOrderRowOption);

            // Properties
            this.Property(t => t.OptionDescription)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(32);

            //this.Property(t => t.additional_locations_emails)
            //    .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("AdditionalLocations");
            this.Property(t => t.idOrderRowOption).HasColumnName("idOrderRowOption");
            this.Property(t => t.idOrderRow).HasColumnName("idOrderRow");
            this.Property(t => t.idRegType).HasColumnName("idRegType");
            this.Property(t => t.RegTypePrice).HasColumnName("RegTypePrice");
            this.Property(t => t.OptionDescription).HasColumnName("OptionDescription");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.Type).HasColumnName("Type");
            //this.Property(t => t.additional_locations_count).HasColumnName("additional_locations_count");
            //this.Property(t => t.additional_locations_emails).HasColumnName("additional_locations_emails");

            // Relationships
            //this.HasRequired(t => t.RegType)
            //    .WithMany(t => t.AdditionalLocations)
            //    .HasForeignKey(d => d.idRegType);
            this.HasRequired(t => t.OrderRow)
                .WithMany(t => t.AdditionalLocations)
                .HasForeignKey(d => d.idOrderRow);

        }
    }
}
