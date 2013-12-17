using System;

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
        public string Type { get; set; }
        public Nullable<int> additional_locations_count { get; set; }
        public string additional_locations_emails { get; set; }
        public virtual Option Option { get; set; }
        public virtual OrderRow OrderRow { get; set; }
    }
}
