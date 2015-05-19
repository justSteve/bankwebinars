using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizUserOrderMap : EntityTypeConfiguration<QuizUserOrder>
    {
        public QuizUserOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.Email).IsRequired().HasMaxLength(150);
            this.Property(t => t.idOrder).IsRequired();
            this.Property(t => t.idQuiz).IsRequired();
            this.Property(t => t.Score).IsRequired();
            this.Property(t => t.QuestionCount).IsRequired();
            this.Property(t => t.DateQuizTaken).IsRequired();

            // Table & Column Mappings
            this.ToTable("QuizUserOrder");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idQuiz).HasColumnName("idQuiz");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Score).HasColumnName("Score");
            this.Property(t => t.QuestionCount).HasColumnName("QuestionCount");
            this.Property(t => t.DateQuizTaken).HasColumnName("DateQuizTaken");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany()
                .HasForeignKey(d => d.idOrder);
            this.HasRequired(t => t.Quiz)
                .WithMany(t => t.QuizUserOrders)
                .HasForeignKey(d => d.idQuiz);
            
        }
    }
}
