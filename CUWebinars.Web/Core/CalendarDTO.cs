using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Core
{
    public class CalendarDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public long start { get; set; }
        public long end { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string learnCaption { get; set; }
        public string learnBody { get; set; }
        public string whoattend { get; set; }
        public string presenter { get; set; }
        public string ceu { get; set; }
        public string pricing { get; set; }
        
    }
}
