using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizUserAnswerMap : EntityTypeConfiguration<QuizUserAnswer>
    {
        public QuizUserAnswerMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.idUser).IsRequired();
            Property(t => t.idQuizQuestion).IsRequired();
            Property(t => t.Letter).HasMaxLength(1)
                .IsFixedLength()
                .IsUnicode(false)
                .IsRequired();

            // Table & Column Mappings
            ToTable("QuizUserAnswer");

            // Relationships
            HasRequired(t => t.WebUser)
                .WithMany()
                .HasForeignKey(t => t.idUser);

            HasRequired(t => t.QuizWithQuestion)
                .WithMany()
                .HasForeignKey(t => t.idQuizQuestion);

        }
    }
}
