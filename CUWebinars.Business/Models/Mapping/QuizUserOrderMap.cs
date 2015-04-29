using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class QuizUserOrderMap : EntityTypeConfiguration<QuizUserOrder>
    {
        public QuizUserOrderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.idUser).IsRequired();
            Property(t => t.idOrder).IsRequired();
            Property(t => t.idQuiz).IsRequired();

            // Table & Column Mappings
            ToTable("QuizUserOrder");

            // Relationships
            HasRequired(t => t.Order)
                .WithMany()
                .HasForeignKey(t => t.idOrder);

            HasRequired(t => t.Quiz)
                .WithMany()
                .HasForeignKey(t => t.idQuiz);
            
            HasRequired(t => t.WebUser)
                .WithMany()
                .HasForeignKey(t => t.idUser);
            
        }
    }
}
