using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class WebinarFile
    {
        public int idWebinarFile { get; set; }
        public int idWebinar { get; set; }
        public string fileLocation { get; set; }
        public string fileDesc { get; set; }
        public virtual Webinar Webinar { get; set; }
        
        
    }
}
