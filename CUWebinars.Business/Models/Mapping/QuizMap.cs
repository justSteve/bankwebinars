using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizMap : EntityTypeConfiguration<Quiz>
    {
        public QuizMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.idWebinar).IsRequired();
            Property(t => t.idTimeLimit).IsOptional();

            // Table & Column Mappings
            ToTable("Quiz");

            // Relationships
            HasRequired(t => t.Webinar)
                .WithMany()
                .HasForeignKey(t => t.idWebinar);
            HasMany(t => t.QuizUserOrders)
                .WithRequired(t => t.Quiz)
                .HasForeignKey(t => t.idQuiz);
            HasMany(t => t.QuizWithQuestions)
                .WithRequired(t => t.Quiz)
                .HasForeignKey(t => t.idQuiz);
        }
    }
}
