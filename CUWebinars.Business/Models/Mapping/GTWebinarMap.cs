using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class GTWebinarMap : EntityTypeConfiguration<GTWebinar>
    {
        public GTWebinarMap()
        {
            //  Need to make decisions above field sizes etc.
            //HasKey(t => t.idUserAff);


            ToTable("GTWebinar");
            //HasRequired(t => t.Webinar)
            //    .WithRequiredDependent(t => t.GTWebinar);
            //.HasForeignKey(d => d.);
        }
    }
}
