using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AffiliateMap : EntityTypeConfiguration<Affiliate>
    {
        public AffiliateMap()
        {
            // Primary Key
            this.HasKey(t => t.idUserAff);

            // Properties
            this.Property(t => t.idUserAff)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.URL)
                .HasMaxLength(500);

            this.Property(t => t.WebBanner)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.WebFooter)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EmailBanner)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EmailFooter)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.ttsDomain)
                .HasMaxLength(50);

            this.Property(t => t.GAPass)
                .HasMaxLength(50);

            this.Property(t => t.supportEmail)
                .HasMaxLength(75);

            this.Property(t => t.DisplayTitle)
                .HasMaxLength(150);

            this.Property(t => t.BillingModel)
                .HasMaxLength(150);

            this.Property(t => t.Logo)
                .HasMaxLength(150);

            this.Property(t => t.ContactPerson)
                .HasMaxLength(150);

            this.Property(t => t.ContactPhone)
                .HasMaxLength(150);

            this.Property(t => t.ContactEmail)
                .HasMaxLength(150);

            this.Property(t => t.ContactFax)
                .HasMaxLength(150);

            this.Property(t => t.ContactAddress)
                .HasMaxLength(150);

            this.Property(t => t.TechEmail)
                .HasMaxLength(150);

            this.Property(t => t.TechPhone)
                .HasMaxLength(150);

            this.Property(t => t.TechName)
                .HasMaxLength(150);

            this.Property(t => t.EmailPromo)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Affiliate");
            this.Property(t => t.idUserAff).HasColumnName("idUserAff");
            this.Property(t => t.CommissionModel).HasColumnName("CommissionModel");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.WebBanner).HasColumnName("WebBanner");
            this.Property(t => t.WebFooter).HasColumnName("WebFooter");
            this.Property(t => t.EmailBanner).HasColumnName("EmailBanner");
            this.Property(t => t.EmailFooter).HasColumnName("EmailFooter");
            this.Property(t => t.ttsDomain).HasColumnName("ttsDomain");
            this.Property(t => t.GAPass).HasColumnName("GAPass");
            this.Property(t => t.supportEmail).HasColumnName("supportEmail");
            this.Property(t => t.DisplayTitle).HasColumnName("DisplayTitle");
            this.Property(t => t.BillingModel).HasColumnName("BillingModel");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            this.Property(t => t.ContactEmail).HasColumnName("ContactEmail");
            this.Property(t => t.ContactFax).HasColumnName("ContactFax");
            this.Property(t => t.ContactAddress).HasColumnName("ContactAddress");
            this.Property(t => t.TechEmail).HasColumnName("TechEmail");
            this.Property(t => t.TechPhone).HasColumnName("TechPhone");
            this.Property(t => t.TechName).HasColumnName("TechName");
            this.Property(t => t.EmailPromo).HasColumnName("EmailPromo");
            this.Property(t => t.WebUser_Id).HasColumnName("WebUser_Id");
            this.Ignore(t => t.State);
            
            // Relationships
            this.HasRequired(t => t.WebUser)
                .WithOptional(t => t.Affiliate);

        }
    }
}
