using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegTypesGroup
    {
        public RegTypesGroup()
        {
            RegTypesGroupsXrefs = new List<RegTypesGroupsXref>();
            RegTypesXrefs = new List<RegTypesXref>();
        }

        public int idRegTypeGroup { get; set; }
        public string RegTypeGroupDesc { get; set; }
        public string RegTypeType { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public virtual ICollection<RegTypesGroupsXref> RegTypesGroupsXrefs { get; set; }
        public virtual ICollection<RegTypesXref> RegTypesXrefs { get; set; }
    }
}
