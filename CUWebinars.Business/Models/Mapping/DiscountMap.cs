using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class DiscountMap : EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            // Primary Key
            this.HasKey(t => t.idDiscount);

            // Properties
            this.Property(t => t.code)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Discount");
            this.Property(t => t.idDiscount).HasColumnName("idDiscount");
            this.Property(t => t.discountType).HasColumnName("discountType");
            this.Property(t => t.code).HasColumnName("code");
            this.Property(t => t.percentOff).HasColumnName("percentOff");
            this.Property(t => t.flatOff).HasColumnName("flatOff");
            this.Property(t => t.usesNumber).HasColumnName("usesNumber");
            this.Property(t => t.dateValidFrom).HasColumnName("dateValidFrom");
            this.Property(t => t.dateValidTo).HasColumnName("dateValidTo");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.dateBilled).HasColumnName("dateBilled");
            this.Property(t => t.cost).HasColumnName("cost");
            this.Property(t => t.Notes).HasColumnName("Notes");
        }
    }
}
