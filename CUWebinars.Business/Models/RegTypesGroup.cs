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
        //Alternative state for given group -- if current group is 'PostEvent' this value will
        // hold the ID of the 'PreEvent' state of the given webinar.
        // Permits us to display all of the RegTypes that are possible for any given webinar
        // without regard to current state.
        public int AltState { get; set; }
        public string RegTypeGroupDesc { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public virtual ICollection<RegTypesGroupsXref> RegTypesGroupsXrefs { get; set; }
        public virtual ICollection<RegTypesXref> RegTypesXrefs { get; set; }
        
        
    }
}
