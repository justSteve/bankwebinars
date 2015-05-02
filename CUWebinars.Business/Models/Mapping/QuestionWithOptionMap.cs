using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionWithOptionMap : EntityTypeConfiguration<QuestionWithOption>
    {
        public QuestionWithOptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.idQuestion).IsRequired();
            Property(t => t.idOption).IsRequired();
            Property(t => t.CorrectAnswer).IsRequired();
            Property(t => t.Letter).HasMaxLength(1)
                .IsFixedLength()
                .IsUnicode(false)
                .IsRequired();

            // Table & Column Mappings
            ToTable("QuestionWithOption");

            // Relationships
            HasRequired(t => t.Option)
                .WithMany()
                .HasForeignKey(t => t.idOption);

            HasRequired(t => t.Question)
                .WithMany(t => t.QuestionWithOptions);

        }
    }
}
