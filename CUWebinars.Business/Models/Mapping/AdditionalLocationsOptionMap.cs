using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AdditionalLocationsOptionMap : EntityTypeConfiguration<AdditionalLocationsOption>
    {
        public AdditionalLocationsOptionMap()
        {
            Property(t => t.AdditionalLocationsCount)
                .IsOptional();

            ToTable("OrderRowOptions");

            Property(t => t.AdditionalLocationsCount).HasColumnName("AdditionalLocationsCount");
        }
    }
}
