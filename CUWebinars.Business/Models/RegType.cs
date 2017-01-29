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
                //return this.OptionLabel.Replace(" Package", "").Replace("Live Plus Six", "Live+6").Replace(" and Hardcopy Handouts", "").Replace(" Recording Only", "").Replace(" Plus Five", "+5");
                // from EditOrder_Compact.cshtml...
                var abbvLable = "";
                if (this.OptionLabel.StartsWith("Live Plus Five"))
                {
                    abbvLable = "Live";
                }                
                if (this.OptionLabel.StartsWith("Live+5"))
                {
                    abbvLable = "Live";
                }
                if (this.OptionLabel.StartsWith("Live Plus Six"))
                {
                    abbvLable = "Live Plus OnDemand";
                }
                if (this.OptionLabel.StartsWith("OnDemand"))
                {
                    abbvLable = "OnDemand";
                }
                if (this.OptionLabel.StartsWith("6-"))
                {
                    abbvLable = "OnDemand";
                }
                if (this.OptionLabel.StartsWith("CD"))
                {
                    abbvLable = "CD-ROM";
                }
                if (this.OptionLabel.StartsWith("Premier"))
                {
                    abbvLable = "Premier";
                }

                return abbvLable;
            }
        }
        public decimal CreditCost { get; set; }
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
