using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class HostPropertyValue
    {
        public int idHostPropertyValue { get; set; }
        public int idHostProperty { get; set; }
        public int idWebinar { get; set; }
        public string value { get; set; }
        public virtual HostProperty HostProperty { get; set; }
        public virtual Webinar Webinar { get; set; }
    }
}
