using System.Collections.Generic;


namespace CUWebinars.Business.Models
{
    //

    public partial class AdditionalLocation : IObjectWithState
    {
        public int idAdditionalLocation { get; set; }
        public int idOrderRow { get; set; }
        public decimal Price { get; set; }
        //Description to use in Cart
        public string DescriptionPromo { get; set; }
        //Description to use after purchased
        public string DescriptionConfirm { get; set; }
        public bool TaxExempt { get; set; }
        public virtual OrderRow OrderRow { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool Billable { get; set; }
        public State DomainEntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
