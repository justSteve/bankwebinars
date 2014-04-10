using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class WebinarTopicXref : IObjectWithState
    {
        public int idWebinarTopicXref { get; set; }
        public int idWebinar { get; set; }
        public int idTopic { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual Webinar Webinar { get; set; }
        public State EntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
