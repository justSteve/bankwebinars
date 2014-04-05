using System.Collections.Generic;


namespace CUWebinars.Business.Models
{
    //

    public partial class AdditionalLocations
    {
        public int Id { get; set; }
        public int idOrderRow { get; set; }
        public decimal Price { get; set; }
        //Description to use in Cart
        public string DescriptionPromo { get; set; }
        //Description to use after purchased
        public string DescriptionConfirm { get; set; }
        public bool TaxExempt { get; set; }
        public virtual OrderRow OrderRow { get; set; }
    }
}
