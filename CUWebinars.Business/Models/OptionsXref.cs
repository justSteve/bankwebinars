using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OptionsXref
    {
        public int idOptionsXref { get; set; }
        public int idOptionGroup { get; set; }
        public int idOption { get; set; }
        public virtual Option Option { get; set; }
        public virtual OptionsGroup OptionsGroup { get; set; }
    }
}
