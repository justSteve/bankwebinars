using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizWithQuestionMap : EntityTypeConfiguration<QuizWithQuestion>
    {
        public QuizWithQuestionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.idQuestion).IsRequired();
            Property(t => t.idQuiz).IsRequired();
            Property(t => t.QuestionNumber).IsRequired();

            // Table & Column Mappings
            ToTable("QuizWithQuestion");

            // Relationships
            HasRequired(t => t.Quiz)
                .WithMany()
                .HasForeignKey(t => t.idQuiz);

            HasRequired(t => t.Question)
                .WithMany()
                .HasForeignKey(t => t.idQuestion);

        }
    }
}
