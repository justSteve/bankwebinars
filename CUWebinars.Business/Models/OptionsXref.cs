using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OptionsXref
    {
        public int idRegTypesXref { get; set; }
        public int idRegTypeGroup { get; set; }
        public int idRegType { get; set; }
        public virtual RegType RegType { get; set; }
        public virtual OptionsGroup OptionsGroup { get; set; }
    }
}
