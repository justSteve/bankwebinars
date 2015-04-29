using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class TimeLimitMap : EntityTypeConfiguration<TimeLimit>
    {
        public TimeLimitMap()
        {
            // Primary Key
            HasKey(t => t.Id);

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
