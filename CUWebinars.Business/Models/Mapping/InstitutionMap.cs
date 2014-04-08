
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class InstitutionMap : EntityTypeConfiguration<Institution>
    {
        public InstitutionMap()
        {
            // Primary Key
            HasKey(t => t.idInstitution);

            // Properties
            Property(t => t.InstitutionName)
                            .IsRequired()
                            .HasMaxLength(250);

            Property(t => t.InstitutionType)
                            .IsRequired()
                            .HasMaxLength(20);

            Property(t => t.domainName)
                            .HasMaxLength(75);

            Property(t => t.RegIdentifier)
                            .HasMaxLength(40);

            // Table & Column Mappings
            ToTable("Institution");
            Property(t => t.idInstitution).HasColumnName("idInstitution");
            Property(t => t.InstitutionName).HasColumnName("InstitutionName");
            Property(t => t.InstitutionType).HasColumnName("InstitutionType");
            Property(t => t.domainName).HasColumnName("domainName");
            Property(t => t.RegIdentifier).HasColumnName("RegIdentifier");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.State).HasColumnName("State");
            Property(t => t.Zip).HasColumnName("Zip");
        }
    }
}
