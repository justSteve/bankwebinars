
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class TopicMap : EntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            // Primary Key
            HasKey(t => t.idTopic);

            // Properties
            Property(t => t.topicDesc)
                            .IsRequired()
                            .HasMaxLength(50);

            Property(t => t.topicHTML)
                            .IsRequired()
                            .HasMaxLength(255);

            // Table & Column Mappings
            ToTable("Topic");
            Property(t => t.idTopic).HasColumnName("idTopic");
            Property(t => t.topicDesc).HasColumnName("topicDesc");
            Property(t => t.idParentTopic).HasColumnName("idParentTopic");
            Property(t => t.topicHTML).HasColumnName("topicHTML");
            Property(t => t.sortOrder).HasColumnName("sortOrder");

            // Relationships
            HasOptional(t => t.Topic2)
                            .WithMany(t => t.Topic1)
                            .HasForeignKey(d => d.idParentTopic);

        }
    }
}
