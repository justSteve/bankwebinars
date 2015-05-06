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

            // Table & Column Mappings
            this.ToTable("QuizUserOrder");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idQuiz).HasColumnName("idQuiz");
            this.Property(t => t.Email).HasColumnName("Email");

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
