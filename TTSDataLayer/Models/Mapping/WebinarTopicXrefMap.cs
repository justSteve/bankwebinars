using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarTopicXrefMap : EntityTypeConfiguration<WebinarTopicXref>
    {
        public WebinarTopicXrefMap()
        {
            // Primary Key
            this.HasKey(t => t.idWebinarTopicXref);

            // Properties
            // Table & Column Mappings
            this.ToTable("WebinarTopicXref");
            this.Property(t => t.idWebinarTopicXref).HasColumnName("idWebinarTopicXref");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.idTopic).HasColumnName("idTopic");

            // Relationships
            this.HasRequired(t => t.Topic)
                .WithMany(t => t.WebinarTopicXrefs)
                .HasForeignKey(d => d.idTopic);
            this.HasRequired(t => t.Webinar)
                .WithMany(t => t.WebinarTopicXrefs)
                .HasForeignKey(d => d.idWebinar);

        }
    }
}
