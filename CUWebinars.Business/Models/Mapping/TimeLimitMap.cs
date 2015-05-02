using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class TimeLimitMap : EntityTypeConfiguration<TimeLimit>
    {
        public TimeLimitMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.Minutes).IsRequired();
            Property(t => t.Hours).IsRequired();
            Property(t => t.Seconds).IsRequired();

            // Table & Column Mappings
            ToTable("TimeLimit");

            // Relationships
        }
    }
}
