using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegType
    {
        public RegType()
        {
            //this.OptionsXrefs = new List<OptionsXref>();
            //this.AdditionalLocation = new List<AdditionalLocation>();
        }

        public int idRegType { get; set; }
        public string OptionExplain { get; set; }
        public string OptionLabel { get; set; }
        public string OptionLabelShort
        {
            get
            {
                return this.OptionLabel.Replace(" and Hardcopy Handouts", "").Replace("Plus Five", "");
                // from EditOrder_Compact.cshtml...
                //if (Model.EditFields.DisplayRowPriceViewModel.RegistrationType.OptionLabel.StartsWith("Live Plus Five"))
                //{
                //    abbvLable = "Live";
                //}
                //if (Model.EditFields.DisplayRowPriceViewModel.RegistrationType.OptionLabel.StartsWith("OnDemand"))
                //{
                //    abbvLable = "Recorded Only";
                //}
                //if (Model.EditFields.DisplayRowPriceViewModel.RegistrationType.OptionLabel.StartsWith("CD"))
                //{
                //    abbvLable = "CD-ROM";
                //}
            }
        }
        public double Price { get; set; }
        public Nullable<bool> TaxExempt { get; set; }
        public int SortOrder { get; set; }
        public string SKU { get; set; }
        public string ShowLiveNotifications { get; set; }
        public string ShowRecordingNotifications { get; set; }
        public string ShowShippedNotifications { get; set; }
        public string Stage1CheckoutConfirmationMsg { get; set; }
        public string Stage2CheckoutConfirmationMsg { get; set; }
        public string Stage1EmailConfirmationMsg { get; set; }
        public string Stage2EmailConfirmationMsg { get; set; }
        //public virtual ICollection<OptionsXref> OptionsXrefs { get; set; }
        //public virtual ICollection<AdditionalLocation> AdditionalLocation { get; set; }
        
        
    }
}
