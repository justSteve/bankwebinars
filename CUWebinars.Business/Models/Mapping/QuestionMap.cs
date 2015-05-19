using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionMap : EntityTypeConfiguration<Question>
    {
        public QuestionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Text)
                .IsRequired()
                .HasMaxLength(800);

            // Table & Column Mappings
            this.ToTable("Question");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Text).HasColumnName("Text");
            this.Property(t => t.idTimeLimit).HasColumnName("idTimeLimit");
            this.Property(t => t.idType).HasColumnName("idType");

            // Relationships
            HasMany(t => t.QuestionWithOptions)
                .WithRequired(t => t.Question)
                .HasForeignKey(t => t.idQuestion);
            HasMany(t => t.QuizWithQuestions)
                .WithRequired(t => t.Question)
                .HasForeignKey(t => t.idQuestion);

        }
    }
}
