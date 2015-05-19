using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuestionWithOptionMap : EntityTypeConfiguration<QuestionWithOption>
    {
        public QuestionWithOptionMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Letter)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("QuestionWithOption");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.idQuestion).HasColumnName("idQuestion");
            this.Property(t => t.idOption).HasColumnName("idOption");
            this.Property(t => t.CorrectAnswer).HasColumnName("CorrectAnswer");
            this.Property(t => t.Letter).HasColumnName("Letter");

            // Relationships
            this.HasRequired(t => t.Option)
                .WithMany(t => t.QuestionWithOptions)
                .HasForeignKey(t => t.idOption);
            this.HasRequired(t => t.Question)
                .WithMany(t => t.QuestionWithOptions)
                .HasForeignKey(t => t.idQuestion);

        }
    }
}
