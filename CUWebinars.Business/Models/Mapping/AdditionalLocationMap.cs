using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AdditionalLocationMap : EntityTypeConfiguration<AdditionalEmails>
    {
        public AdditionalLocationMap()
        {
            //  Primary Key
            HasKey(a => a.Id);

            // Properties
            // Table & Column Mappings
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Email).HasColumnName("Email").IsRequired().IsVariableLength().HasMaxLength(150);
            Property(t => t.FullName).HasColumnName("FullName").IsOptional().IsVariableLength().HasMaxLength(100);
            Property(t => t.Id).HasColumnName("idOrderRow").IsRequired();

            // Relationships
            //HasRequired(a => a.AdditionalLocations)
            //    .WithMany(o => o.A)
            //    .HasForeignKey(a => a.idOrderRow);
        }
    }
}
