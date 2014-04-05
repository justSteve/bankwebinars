using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OptionsGroup
    {
        public OptionsGroup()
        {
            this.OptionsGroupsXrefs = new List<OptionsGroupsXref>();
            this.OptionsXrefs = new List<OptionsXref>();
        }

        public int idRegTypeGroup { get; set; }
        public string OptionGroupDesc { get; set; }
        public string OptionType { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public virtual ICollection<OptionsGroupsXref> OptionsGroupsXrefs { get; set; }
        public virtual ICollection<OptionsXref> OptionsXrefs { get; set; }
    }
}
