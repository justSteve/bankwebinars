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


            // Table & Column Mappings
            this.ToTable("QuizUserOrder");
            this.Property(t => t.Id).HasColumnName("Id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            this.Property(t => t.idUser).HasColumnName("idUser");
            this.Property(t => t.idOrder).HasColumnName("idOrder");
            this.Property(t => t.idQuiz).HasColumnName("idQuiz");

            // Relationships
            this.HasRequired(t => t.Order)
                .WithMany()
                .HasForeignKey(d => d.idOrder);
            this.HasRequired(t => t.Quiz)
                .WithMany(t => t.QuizUserOrders)
                .HasForeignKey(d => d.idQuiz);
            this.HasRequired(t => t.WebUser)
                .WithMany()
                .HasForeignKey(d => d.idUser);

        }
    }
}
