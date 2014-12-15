
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
            Property(t => t.code)
                            .IsRequired()
                            .HasMaxLength(50);

            Property(t => t.status)
                            .IsRequired()
                            .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Discount");
            Property(t => t.idDiscount).HasColumnName("idDiscount");
            Property(t => t.discountType).IsRequired();
            Property(t => t.code).HasColumnName("code");
            Property(t => t.percentOff).HasColumnName("percentOff");
            Property(t => t.flatOff).HasColumnName("flatOff");
            Property(t => t.usesNumber).HasColumnName("usesNumber");
            Property(t => t.dateValidFrom).HasColumnName("dateValidFrom");
            Property(t => t.dateValidTo).HasColumnName("dateValidTo");
            Property(t => t.status).HasColumnName("status");
            Property(t => t.dateBilled).HasColumnName("dateBilled");
            Property(t => t.cost).HasColumnName("cost");
            Property(t => t.Notes).HasColumnName("Notes");
            Property(t => t.renewalTerm).HasColumnName("renewalTerm");

        }
    }
}
