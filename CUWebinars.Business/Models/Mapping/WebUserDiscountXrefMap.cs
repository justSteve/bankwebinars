using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebUserDiscountXrefMap : EntityTypeConfiguration<WebUserDiscountXref>
    {
        public WebUserDiscountXrefMap()
        {
            // Primary Key
            HasKey(t => t.idWebUserDiscount);

            // Properties
            // Table & Column Mappings
            ToTable("WebUserDiscountXref");

            Property(t => t.idWebUserDiscount).HasColumnName("idWebUserDiscount");
            Property(t => t.idWebUser).HasColumnName("idWebUser");
            Property(t => t.idDiscount).HasColumnName("idDiscount");

            // Relationships
            HasRequired(t => t.Discount)
                .WithRequiredDependent(t => t.WebUserDiscountXref);
            HasRequired(t => t.WebUser)
                .WithRequiredDependent(t => t.WebUserDiscountXref);
        }
    }
}
