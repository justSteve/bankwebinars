
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebUserMap : EntityTypeConfiguration<WebUser>
    {
        public WebUserMap()
        {
            // Primary Key
            HasKey(t => t.idUser);

            // Properties
            Property(t => t.idUser)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AcctStatus)
                            .IsRequired()
                            .HasMaxLength(1);

            Property(t => t.FirstName)
                            .IsRequired()
                            .HasMaxLength(50);

            Property(t => t.LastName)
                            .IsRequired()
                            .HasMaxLength(50);

            Property(t => t.email)
                            .IsRequired()
                            .HasMaxLength(150);

            Property(t => t.futureMail)
                            .HasMaxLength(1);

            Property(t => t.generalComments)
                            .HasMaxLength(1000);

            Property(t => t.Title)
                            .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("WebUser");
            Property(t => t.idUser).HasColumnName("idUser");
            Property(t => t.UserType).HasColumnName("UserType");
            Property(t => t.AcctStatus).HasColumnName("AcctStatus");
            Property(t => t.DateCreated).HasColumnName("DateCreated");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.idUserInstitution).HasColumnName("idUserInstitution");
            Property(t => t.email).HasColumnName("email");
            Property(t => t.futureMail).HasColumnName("futureMail");
            Property(t => t.generalComments).HasColumnName("generalComments");
            Property(t => t.taxExempt).HasColumnName("taxExempt");
            Property(t => t.idSubscriptionDiscount).HasColumnName("idSubscriptionDiscount");
            Property(t => t.timeZone).HasColumnName("timeZone");
            Property(t => t.Title).HasColumnName("Title");

            Ignore(t => t.DomainEntityState);
            Ignore(t => t.OriginalValues);


            // Relationships
            HasRequired(t => t.Institution)
                            .WithMany(t => t.WebUsers)
                            .HasForeignKey(d => d.idUserInstitution);

        }
    }
}
