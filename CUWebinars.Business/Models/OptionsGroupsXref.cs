using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OptionsGroupsXref
    {
        public int idWebinarOptionGroup { get; set; }
        public int idWebinar { get; set; }
        public int idOptionGroup { get; set; }
        public virtual OptionsGroup OptionsGroup { get; set; }
        public virtual Webinar Webinar { get; set; }
    }
}
