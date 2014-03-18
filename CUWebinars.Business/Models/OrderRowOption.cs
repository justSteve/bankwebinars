using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OrderRowOption
    {
        public int idOrderRowOption { get; set; }
        public int idOrderRow { get; set; }
        public int idOption { get; set; }
        public decimal OptionPrice { get; set; }
        public string OptionDescription { get; set; }
        public bool TaxExempt { get; set; }
        public string Type { get; set; }//  Looks like the Type property is being used in SetAdditionalLocations method of RegistrationsController
        //public string additional_locations_emails { get; set; }   See new entity AdditionalLocation with FK back to this. 
        public virtual Option Option { get; set; }
        public virtual OrderRow OrderRow { get; set; }

        public virtual ICollection<AdditionalLocation> AdditionalLocations { get; set; }
    }
}
