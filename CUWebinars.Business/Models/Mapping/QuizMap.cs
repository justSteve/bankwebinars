using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
        public class QuizMap : EntityTypeConfiguration<Quiz>
    {
            public QuizMap()
            {
                // Primary Key
                HasKey(t => t.Id);
                Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

                // Properties
                Property(t => t.idWebinar).IsRequired();
                Property(t => t.idTimeLimit).IsOptional();

                // Table & Column Mappings
                ToTable("Quiz");

                // Relationships
                HasRequired(t => t.Webinar)
                    .WithMany()
                    .HasForeignKey(t => t.idWebinar);

                HasOptional(t => t.TimeLimit)
                    .WithMany()
                    .HasForeignKey(t => t.idTimeLimit);

            }
    }
}
