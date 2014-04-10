using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class WebinarFile : IObjectWithState
    {
        public int idWebinarFile { get; set; }
        public int idWebinar { get; set; }
        public string fileLocation { get; set; }
        public string fileDesc { get; set; }
        public virtual Webinar Webinar { get; set; }
        public State DomainEntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
