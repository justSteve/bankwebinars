using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarMap : EntityTypeConfiguration<Webinar>
    {
        public WebinarMap()
        {
            // Primary Key
            this.HasKey(t => t.idWebinar);

            // Properties
            this.Property(t => t.Description)
                .IsRequired();

            this.Property(t => t.DescriptionLong)
                .IsRequired();

            this.Property(t => t.ImageUrl)
                .HasMaxLength(50);

            this.Property(t => t.SmallImageUrl)
                .HasMaxLength(50);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(125);

            this.Property(t => t.LearnCaption)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.LearnBody)
                .IsRequired();

            this.Property(t => t.WhoAttend)
                .IsRequired();

            this.Property(t => t.RecordingUrl)
                .IsRequired()
                .HasMaxLength(300);

            this.Property(t => t.ceu)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("Webinar");
            this.Property(t => t.idWebinar).HasColumnName("idWebinar");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.DescriptionLong).HasColumnName("DescriptionLong");
            this.Property(t => t.ImageUrl).HasColumnName("ImageUrl");
            this.Property(t => t.SmallImageUrl).HasColumnName("SmallImageUrl");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.LearnCaption).HasColumnName("LearnCaption");
            this.Property(t => t.LearnBody).HasColumnName("LearnBody");
            this.Property(t => t.WhoAttend).HasColumnName("WhoAttend");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.RecordingUrl).HasColumnName("RecordingUrl");
            this.Property(t => t.ConnectionInfo).HasColumnName("ConnectionInfo");
            this.Property(t => t.idPresenter).HasColumnName("idPresenter");
            this.Property(t => t.AdditionalNotifications).HasColumnName("AdditionalNotifications");
            this.Property(t => t.ceu).HasColumnName("ceu");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.DateChanged).HasColumnName("DateChanged");

            // Relationships
            this.HasRequired(t => t.Presenter)
                .WithMany(t => t.Webinars)
                .HasForeignKey(d => d.idPresenter);

        }
    }
}
