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
                if (OptionLabel.StartsWith("Live Plus Five"))
                {
                    return "Live";
                }
                if (OptionLabel.StartsWith("Live+5"))
                {
                    return "Live";
                }
                if (OptionLabel.StartsWith("Live Plus Six"))
                {
                    return "Live Plus OnDemand";
                }
                if (OptionLabel.StartsWith("OnDemand"))
                {
                    return "OnDemand";
                }
                if (OptionLabel.StartsWith("6-"))
                {
                    return "6-Month";
                }
                if (OptionLabel.StartsWith("12-"))
                {
                    return "12-Month";
                }
                if (OptionLabel.StartsWith("CD"))
                {
                    return "CD-ROM";
                }
                if (OptionLabel.StartsWith("Premier"))
                {
                    return "Premier";
                }
                if (OptionLabel.StartsWith("Non"))
                {
                    return "Non-bank or < 499M";
                }

                if (OptionLabel.StartsWith("$50"))
                {
                    return "$500M - 999M";
                }

                if (OptionLabel.StartsWith("$1 "))
                {
                    return "$1 - 5B";
                }

                if (OptionLabel.StartsWith("$5 "))
                {
                    return "$5 - 10B";
                }

                if (OptionLabel.StartsWith(">"))
                {
                    return "> 10B - $850";
                }
                return OptionLabel;

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
