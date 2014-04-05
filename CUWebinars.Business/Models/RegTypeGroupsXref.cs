using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegTypesGroupsXref
    {
        public int idWebinarRegTypeGroup { get; set; }
        public int idWebinar { get; set; }
        public int idRegTypeGroup { get; set; }
        public virtual RegTypesGroup RegTypesGroup { get; set; }
        public virtual Webinar Webinar { get; set; }
    }
}
