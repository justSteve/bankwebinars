using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionMap : EntityTypeConfiguration<Question>
    {
        public QuestionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.Text).HasMaxLength(800).IsRequired();
            Property(t => t.QuestionType).HasColumnName("idType");

            // Table & Column Mappings
            ToTable("Question");

            // Relationships
            HasOptional(t => t.TimeLimit)
                .WithMany()
                .HasForeignKey(t => t.idTimeLimit);

            
        }
    }
}
