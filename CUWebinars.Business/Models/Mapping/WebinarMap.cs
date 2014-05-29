
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class WebinarMap : EntityTypeConfiguration<Webinar>
    {
        public WebinarMap()
        {
            // Primary Key
            HasKey(t => t.idWebinar);

            // Properties
            Property(t => t.Description)
                            .IsRequired();

            Property(t => t.DescriptionLong)
                            .IsRequired();

            Property(t => t.ImageUrl)
                            .HasMaxLength(50);

            Property(t => t.SmallImageUrl)
                            .HasMaxLength(50);

            Property(t => t.Title)
                            .IsRequired()
                            .HasMaxLength(125);

            Property(t => t.LearnCaption)
                            .IsRequired()
                            .HasMaxLength(255);

            Property(t => t.LearnBody)
                            .IsRequired();

            Property(t => t.WhoAttend)
                            .IsRequired();

            Property(t => t.RecordingUrl)
                            .IsRequired()
                            .HasMaxLength(300);

            Property(t => t.ceu)
                            .HasMaxLength(1000);

            // Table & Column Mappings
            ToTable("Webinar");
            Property(t => t.idWebinar).HasColumnName("idWebinar");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.DescriptionLong).HasColumnName("DescriptionLong");
            Property(t => t.ImageUrl).HasColumnName("ImageUrl");
            Property(t => t.SmallImageUrl).HasColumnName("SmallImageUrl");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.LearnCaption).HasColumnName("LearnCaption");
            Property(t => t.LearnBody).HasColumnName("LearnBody");
            Property(t => t.WhoAttend).HasColumnName("WhoAttend");
            Property(t => t.Duration).HasColumnName("Duration");
            Property(t => t.RecordingUrl).HasColumnName("RecordingUrl");
            Property(t => t.ConnectionInfo).HasColumnName("ConnectionInfo");
            Property(t => t.idPresenter).HasColumnName("idPresenter");
            Property(t => t.ceu).HasColumnName("ceu");
            Property(t => t.DateCreated).HasColumnName("DateCreated");
            Property(t => t.DateChanged).HasColumnName("DateChanged");
            


            // Relationships
            HasRequired(t => t.Presenter)
                            .WithMany(t => t.Webinars)
                            .HasForeignKey(d => d.idPresenter);
            
        }
    }
}
