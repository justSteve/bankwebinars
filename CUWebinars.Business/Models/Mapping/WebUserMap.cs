using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebUserMap : EntityTypeConfiguration<WebUser>
    {
        public WebUserMap()
        {
            // Primary Key
            this.HasKey(t => t.idUser);

            // Properties
            this.Property(t => t.idUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AcctStatus)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.email)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.futureMail)
                .HasMaxLength(1);

            this.Property(t => t.generalComments)
                .HasMaxLength(1000);

            this.Property(t => t.Title)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("WebUser");
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.UserType).HasColumnName("UserType");
            this.Property(t => t.AcctStatus).HasColumnName("AcctStatus");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.idUserInstitution).HasColumnName("idUserInstitution");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.futureMail).HasColumnName("futureMail");
            this.Property(t => t.generalComments).HasColumnName("generalComments");
            this.Property(t => t.taxExempt).HasColumnName("taxExempt");
            this.Property(t => t.idSubscriptionDiscount).HasColumnName("idSubscriptionDiscount");
            this.Property(t => t.timeZone).HasColumnName("timeZone");
            this.Property(t => t.Title).HasColumnName("Title");

            // Relationships
            this.HasRequired(t => t.Institution)
                .WithMany(t => t.WebUsers)
                .HasForeignKey(d => d.idUserInstitution);

        }
    }
}
