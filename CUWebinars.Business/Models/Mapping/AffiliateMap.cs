
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AffiliateMap : EntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            // Primary Key
            HasKey(t => t.idUserAff);

            // Properties
            Property(t => t.idUserAff)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.URL)
                            .HasMaxLength(500);

            Property(t => t.WebBanner)
                            .IsRequired()
                            .HasMaxLength(4000);

            Property(t => t.WebFooter)
                            .IsRequired()
                            .HasMaxLength(4000);

            Property(t => t.EmailBanner)
                            .IsRequired()
                            .HasMaxLength(4000);

            Property(t => t.EmailFooter)
                            .IsRequired()
                            .HasMaxLength(4000);

            Property(t => t.ttsDomain)
                            .HasMaxLength(50);

            Property(t => t.GAPass)
                            .HasMaxLength(50);

            Property(t => t.supportEmail)
                            .HasMaxLength(75);

            Property(t => t.DisplayTitle)
                            .HasMaxLength(150);

            Property(t => t.BillingModel)
                            .HasMaxLength(150);

            Property(t => t.Logo)
                            .HasMaxLength(150);

            Property(t => t.ContactPerson)
                            .HasMaxLength(150);

            Property(t => t.ContactPhone)
                            .HasMaxLength(150);

            Property(t => t.ContactEmail)
                            .HasMaxLength(150);

            Property(t => t.ContactFax)
                            .HasMaxLength(150);

            Property(t => t.ContactAddress)
                            .HasMaxLength(150);

            Property(t => t.TechEmail)
                            .HasMaxLength(150);

            Property(t => t.TechPhone)
                            .HasMaxLength(150);

            Property(t => t.TechName)
                            .HasMaxLength(150);

            Property(t => t.EmailPromo)
                            .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Affiliate");
            Property(t => t.idUserAff).HasColumnName("idUserAff");
            Property(t => t.CommissionModel).HasColumnName("CommissionModel");
            Property(t => t.URL).HasColumnName("URL");
            Property(t => t.WebBanner).HasColumnName("WebBanner");
            Property(t => t.WebFooter).HasColumnName("WebFooter");
            Property(t => t.EmailBanner).HasColumnName("EmailBanner");
            Property(t => t.EmailFooter).HasColumnName("EmailFooter");
            Property(t => t.ttsDomain).HasColumnName("ttsDomain");
            Property(t => t.GAPass).HasColumnName("GAPass");
            Property(t => t.supportEmail).HasColumnName("supportEmail");
            Property(t => t.DisplayTitle).HasColumnName("DisplayTitle");
            Property(t => t.BillingModel).HasColumnName("BillingModel");
            Property(t => t.Logo).HasColumnName("Logo");
            Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            Property(t => t.ContactEmail).HasColumnName("ContactEmail");
            Property(t => t.ContactFax).HasColumnName("ContactFax");
            Property(t => t.ContactAddress).HasColumnName("ContactAddress");
            Property(t => t.TechEmail).HasColumnName("TechEmail");
            Property(t => t.TechPhone).HasColumnName("TechPhone");
            Property(t => t.TechName).HasColumnName("TechName");
            Property(t => t.EmailPromo).HasColumnName("EmailPromo");
            Property(t => t.WebUser_Id).HasColumnName("WebUser_Id");
            Ignore(t => t.State);

            // Relationships
            HasRequired(t => t.WebUser)
                            .WithOptional(t => t.Affiliate);

        }
    }
}
