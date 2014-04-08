
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarTopicXrefMap : EntityTypeConfiguration<WebinarTopicXref>
    {
        public WebinarTopicXrefMap()
        {
            // Primary Key
            HasKey(t => t.idWebinarTopicXref);

            // Properties
            // Table & Column Mappings
            ToTable("WebinarTopicXref");
            Property(t => t.idWebinarTopicXref).HasColumnName("idWebinarTopicXref");
            Property(t => t.idWebinar).HasColumnName("idWebinar");
            Property(t => t.idTopic).HasColumnName("idTopic");

            // Relationships
            HasRequired(t => t.Topic)
                            .WithMany(t => t.WebinarTopicXrefs)
                            .HasForeignKey(d => d.idTopic);
            HasRequired(t => t.Webinar)
                            .WithMany(t => t.WebinarTopicXrefs)
                            .HasForeignKey(d => d.idWebinar);

        }
    }
}
