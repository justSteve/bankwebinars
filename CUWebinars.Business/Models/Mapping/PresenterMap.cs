
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class PresenterMap : EntityTypeConfiguration<Presenter>
    {
        public PresenterMap()
        {
            // Primary Key
            HasKey(t => t.idUser);

            // Properties
            Property(t => t.idUser)
                            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Biography)
                            .HasMaxLength(2500);

            Property(t => t.BiographyLong)
                            .IsRequired()
                            .HasMaxLength(2500);

            Property(t => t.PhotoFull)
                            .HasMaxLength(200);

            Property(t => t.PhotoThumb)
                            .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("Presenter");
            Property(t => t.idUser).HasColumnName("idUser");
            Property(t => t.Biography).HasColumnName("Biography");
            Property(t => t.BiographyLong).HasColumnName("BiographyLong");
            Property(t => t.PhotoFull).HasColumnName("PhotoFull");
            Property(t => t.PhotoThumb).HasColumnName("PhotoThumb");

            // Relationships
            HasRequired(t => t.WebUser)
                            .WithOptional(t => t.Presenter);

        }
    }
}
