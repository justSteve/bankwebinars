using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarFileMap : EntityTypeConfiguration<WebinarFile>
    {
        public WebinarFileMap()
        {
            // Primary Key
            this.HasKey(t => t.idWebinarFile);

            // Properties
            this.Property(t => t.fileLocation)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.fileDesc)
                .IsRequired()
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("WebinarFile");
            this.Property(t => t.idWebinarFile).HasColumnName("idWebinarFile");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.fileLocation).HasColumnName("fileLocation");
            this.Property(t => t.fileDesc).HasColumnName("fileDesc");

            // Relationships
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.WebinarFiles)
                .HasForeignKey(d => d.idWebinar);

        }
    }
}
