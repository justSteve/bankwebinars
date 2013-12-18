using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class PresenterMap : EntityTypeConfiguration<Presenter>
    {
        public PresenterMap()
        {
            // Primary Key
            this.HasKey(t => t.idUser);

            // Properties
            this.Property(t => t.idUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Biography)
                .HasMaxLength(140);

            this.Property(t => t.BiographyLong)
                .IsRequired()
                .HasMaxLength(2500);

            this.Property(t => t.PhotoFull)
                .HasMaxLength(200);

            this.Property(t => t.PhotoThumb)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Presenter");
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.Biography).HasColumnName("Biography");
            this.Property(t => t.BiographyLong).HasColumnName("BiographyLong");
            this.Property(t => t.PhotoFull).HasColumnName("PhotoFull");
            this.Property(t => t.PhotoThumb).HasColumnName("PhotoThumb");

            // Relationships
            this.HasRequired(t => t.WebUser)
                .WithOptional(t => t.Presenter);

        }
    }
}
