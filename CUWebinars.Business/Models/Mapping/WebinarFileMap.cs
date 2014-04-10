
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarFileMap : EntityTypeConfiguration<WebinarFile>
    {
        public WebinarFileMap()
        {
            // Primary Key
            HasKey(t => t.idWebinarFile);

            // Properties
            Property(t => t.fileLocation)
                            .IsRequired()
                            .HasMaxLength(255);

            Property(t => t.fileDesc)
                            .IsRequired()
                            .HasMaxLength(1000);

            // Table & Column Mappings
            ToTable("WebinarFile");
            Property(t => t.idWebinarFile).HasColumnName("idWebinarFile");
            Property(t => t.idWebinar).HasColumnName("idWebinar");
            Property(t => t.fileLocation).HasColumnName("fileLocation");
            Property(t => t.fileDesc).HasColumnName("fileDesc");

            Ignore(t => t.EntityState);
            Ignore(t => t.OriginalValues);


            // Relationships
            HasRequired(t => t.Webinar)
                            .WithMany(t => t.WebinarFiles)
                            .HasForeignKey(d => d.idWebinar);

        }
    }
}
