using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionWithOptionMap : EntityTypeConfiguration<QuestionWithOption>
    {
        public QuestionWithOptionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.idQuestion).IsRequired();
            Property(t => t.idOption).IsRequired();
            Property(t => t.CorrectAnswer).IsRequired();
            Property(t => t.Letter).IsRequired();

            // Table & Column Mappings
            ToTable("Question");

            // Relationships
            HasRequired(t => t.Option)
                .WithMany()
                .HasForeignKey(t => t.idOption);

            HasRequired(t => t.Question)
                .WithMany()
                .HasForeignKey(t => t.idQuestion);

        }
    }
}
