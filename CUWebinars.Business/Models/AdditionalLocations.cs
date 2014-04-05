using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CUWebinars.Business.Models
{
    public partial class AdditionalLocations
    {
        public int Id { get; set; }
        public int idOrderRow { get; set; }
        public int idRegType { get; set; }
        public decimal RegTypePrice { get; set; }
        public string OptionDescription { get; set; }
        public bool TaxExempt { get; set; }
        //public string Type { get; set; }//  Looks like the Type property is being used in SetAdditionalLocations method of RegistrationsController
        //public virtual RegType RegType { get; set; }
        public virtual OrderRow OrderRow { get; set; }

    }
}
