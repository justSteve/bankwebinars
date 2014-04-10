using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegTypesGroup : IObjectWithState
    {
        public RegTypesGroup()
        {
            RegTypesGroupsXrefs = new List<RegTypesGroupsXref>();
            RegTypesXrefs = new List<RegTypesXref>();
        }

        public int idRegTypeGroup { get; set; }
        public string RegTypeGroupDesc { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public virtual ICollection<RegTypesGroupsXref> RegTypesGroupsXrefs { get; set; }
        public virtual ICollection<RegTypesXref> RegTypesXrefs { get; set; }
        public State EntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
