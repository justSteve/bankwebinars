using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionMap : EntityTypeConfiguration<Question>
    {
        public QuestionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Text).HasMaxLength(800).IsRequired();
            Property(t => t.QuestionType).IsRequired();

            // Table & Column Mappings
            ToTable("Question");

            // Relationships
            HasOptional(t => t.TimeLimit)
                .WithMany()
                .HasForeignKey(t => t.idTimeLimit);
        }
    }
}
