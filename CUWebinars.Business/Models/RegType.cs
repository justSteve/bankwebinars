using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegType
    {
        public RegType()
        {
            //this.OptionsXrefs = new List<OptionsXref>();
            //this.AdditionalLocations = new List<AdditionalLocations>();
        }

        public int idRegType { get; set; }
        public string OptionExplain { get; set; }
        public string OptionLabel { get; set; }
        public Nullable<double> PriceToAdd { get; set; }
        public Nullable<bool> TaxExempt { get; set; }
        public Nullable<double> PercToAdd { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string Type { get; set; }
        public string MsgConfirm { get; set; }
        public string SKU { get; set; }
        public string ShowLiveNotifications { get; set; }
        public string ShowRecordingNotifications { get; set; }
        public string ShowShippedNotifications { get; set; }
        public string Stage1CheckoutConfirmationMsg { get; set; }
        public string Stage2CheckoutConfirmationMsg { get; set; }
        public string Stage1EmailConfirmationMsg { get; set; }
        public string Stage2EmailConfirmationMsg { get; set; }
        //public virtual ICollection<OptionsXref> OptionsXrefs { get; set; }
        //public virtual ICollection<AdditionalLocations> AdditionalLocations { get; set; }
    }
}
