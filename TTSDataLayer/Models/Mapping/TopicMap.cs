using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class TopicMap : EntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            // Primary Key
            this.HasKey(t => t.idTopic);

            // Properties
            this.Property(t => t.topicDesc)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.topicHTML)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Topic");
            this.Property(t => t.idTopic).HasColumnName("idTopic");
            this.Property(t => t.topicDesc).HasColumnName("topicDesc");
            this.Property(t => t.idParentTopic).HasColumnName("idParentTopic");
            this.Property(t => t.topicHTML).HasColumnName("topicHTML");
            this.Property(t => t.sortOrder).HasColumnName("sortOrder");

            // Relationships
            this.HasOptional(t => t.Topic2)
                .WithMany(t => t.Topic1)
                .HasForeignKey(d => d.idParentTopic);

        }
    }
}
