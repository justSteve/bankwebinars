
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class DiscountMap : EntityTypeConfiguration<Discount>
    {
        public DiscountMap()
        {
            // Primary Key
            HasKey(t => t.idDiscount);

            // Properties
            Property(t => t.DiscountCode)
                            .IsRequired()
                            .HasMaxLength(50);

            Property(t => t.Status)
                            .IsRequired()
                            .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Discount");
            Property(t => t.idDiscount).HasColumnName("idDiscount");
            Property(t => t.DiscountType).IsRequired();
            Property(t => t.DiscountCode).HasColumnName("DiscountCode");
            Property(t => t.PercentOff).HasColumnName("PercentOff");
            Property(t => t.FlatOff).HasColumnName("FlatOff");
            Property(t => t.UsesCount).HasColumnName("usesCount");
            Property(t => t.UsesRemain).HasColumnName("UsesRemain");
            Property(t => t.DateValidFrom).HasColumnName("dateValidFrom");
            Property(t => t.DateValidTo).HasColumnName("dateValidTo");
            Property(t => t.Status).HasColumnName("status");
            Property(t => t.DateBilled).HasColumnName("dateBilled");
            Property(t => t.Cost).HasColumnName("cost");
            Property(t => t.Notes).HasColumnName("Notes");
            Property(t => t.RenewalTerm).HasColumnName("renewalTerm");

        }
    }
}
