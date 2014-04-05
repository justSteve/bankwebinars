using System.Data.Entity.ModelConfiguration;

namespace CUWebinars.Business.Models.Mapping
{
    public class AdditionalLocationMap : EntityTypeConfiguration<AdditionalEmails>
    {
        public AdditionalLocationMap()
        {

        //            public int Id { get; set; }
        //public int idOrderRow { get; set; }
        //public decimal Price { get; set; }
        //public string DescriptionPromo { get; set; }
        //public string DescriptionConfirm { get; set; }
        //public bool TaxExempt { get; set; }
        //public virtual OrderRow OrderRow { get; set; }
            // Relationships
            //HasRequired(a => a.AdditionalLocations)
            //    .WithMany(o => o.A)
            //    .HasForeignKey(a => a.idOrderRow);
        }
    }
}
