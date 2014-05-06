using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class CitrixWebinarMap : EntityTypeConfiguration<CitrixWebinar>
    {
        public CitrixWebinarMap()
        {
            //  Need to make decisions above field sizes etc.

            ToTable("CitrixWebinar");

        }
    }
}
