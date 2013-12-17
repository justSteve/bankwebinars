using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class OrderRowOptionMap : EntityTypeConfiguration<OrderRowOption>
    {
        public OrderRowOptionMap()
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

            this.Property(t => t.additional_locations_emails)
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("OrderRowOptions");
            this.Property(t => t.idOrderRowOption).HasColumnName("idOrderRowOption");
            this.Property(t => t.idOrderRow).HasColumnName("idOrderRow");
            this.Property(t => t.idOption).HasColumnName("idOption");
            this.Property(t => t.OptionPrice).HasColumnName("OptionPrice");
            this.Property(t => t.OptionDescription).HasColumnName("OptionDescription");
            this.Property(t => t.TaxExempt).HasColumnName("TaxExempt");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.additional_locations_count).HasColumnName("additional_locations_count");
            this.Property(t => t.additional_locations_emails).HasColumnName("additional_locations_emails");

            // Relationships
            this.HasRequired(t => t.Option)
                .WithMany(t => t.OrderRowOptions)
                .HasForeignKey(d => d.idOption);
            this.HasRequired(t => t.OrderRow)
                .WithMany(t => t.OrderRowOptions)
                .HasForeignKey(d => d.idOrderRow);

        }
    }
}
