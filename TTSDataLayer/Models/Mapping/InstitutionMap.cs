using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class InstitutionMap : EntityTypeConfiguration<Institution>
    {
        public InstitutionMap()
        {
            // Primary Key
            this.HasKey(t => t.idInstitution);

            // Properties
            this.Property(t => t.InstitutionName)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.InstitutionType)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.domainName)
                .HasMaxLength(75);

            this.Property(t => t.RegIdentifier)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("Institution");
            this.Property(t => t.idInstitution).HasColumnName("idInstitution");
            this.Property(t => t.InstitutionName).HasColumnName("InstitutionName");
            this.Property(t => t.InstitutionType).HasColumnName("InstitutionType");
            this.Property(t => t.domainName).HasColumnName("domainName");
            this.Property(t => t.RegIdentifier).HasColumnName("RegIdentifier");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.Zip).HasColumnName("Zip");
        }
    }
}
