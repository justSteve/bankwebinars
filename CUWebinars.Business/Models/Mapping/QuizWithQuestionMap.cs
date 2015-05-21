using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizWithQuestionMap : EntityTypeConfiguration<QuizWithQuestion>
    {
        public QuizWithQuestionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.idQuestion).IsRequired();
            Property(t => t.idQuiz).IsRequired();
            Property(t => t.QuestionNumber).IsRequired();

            // Table & Column Mappings
            ToTable("QuizWithQuestion");

            // Relationships
            HasRequired(t => t.Quiz)
                .WithMany(t => t.QuizWithQuestions)
                .HasForeignKey(t => t.idQuiz);
            HasRequired(t => t.Question)
                .WithMany(t => t.QuizWithQuestions)
                .HasForeignKey(t => t.idQuestion);
            HasMany(t => t.QuizUserAnswers)
                .WithOptional(t => t.QuizWithQuestion)
                .HasForeignKey(t => t.idQuizQuestion);
        }
    }
}
