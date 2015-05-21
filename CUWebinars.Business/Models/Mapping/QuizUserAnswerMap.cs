using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizUserAnswerMap : EntityTypeConfiguration<QuizUserAnswer>
    {
        public QuizUserAnswerMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties

            this.Property(t => t.Letter)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);
            this.Property(t => t.idQuizQuestion).IsOptional();
            this.Property(t => t.Email).IsRequired().HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("QuizUserAnswer");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.idQuizQuestion).HasColumnName("idQuizQuestion");
            this.Property(t => t.idQuizUserOrder).HasColumnName("idQuizUserOrder");
            this.Property(t => t.Letter).HasColumnName("Letter");
            this.Property(t => t.Email).HasColumnName("Email");

            // Relationships
            this.HasRequired(t => t.QuizUserOrder)
                .WithMany(t => t.QuizUserAnswers)
                .HasForeignKey(d => d.idQuizUserOrder);
            this.HasOptional(t => t.QuizWithQuestion)
                .WithMany(t => t.QuizUserAnswers)
                .HasForeignKey(d => d.idQuizQuestion);
        }
    }
}
