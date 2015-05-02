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

            // Table & Column Mappings
            this.ToTable("QuizUserAnswer");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.idQuizQuestion).HasColumnName("idQuizQuestion");
            this.Property(t => t.Letter).HasColumnName("Letter");

            // Relationships
            this.HasRequired(t => t.QuizWithQuestion)
                .WithMany(t => t.QuizUserAnswers)
                .HasForeignKey(d => d.idQuizQuestion);
            this.HasRequired(t => t.WebUser)
                .WithMany()
                .HasForeignKey(d => d.idUser);
        }
    }
}
