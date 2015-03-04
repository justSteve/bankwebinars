using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AdditionalLocationsLookupPriceMap : EntityTypeConfiguration<AdditionalLocationsLookupPrice>
    {
        public AdditionalLocationsLookupPriceMap()
        {
            HasKey(a => a.id);

            Property(a=> a.id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            ToTable("AdditionalLocationsLookupPrice");

            // Relationships
            HasRequired(a => a.Webinar).WithRequiredPrincipal();
        }
    }
}
