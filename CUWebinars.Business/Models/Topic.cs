using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Topic : IObjectWithState
    {
        public Topic()
        {
            Topic1 = new List<Topic>();
            WebinarTopicXrefs = new List<WebinarTopicXref>();
        }

        public int idTopic { get; set; }
        public string topicDesc { get; set; }
        public Nullable<int> idParentTopic { get; set; }
        public string topicHTML { get; set; }
        public int sortOrder { get; set; }
        public virtual ICollection<Topic> Topic1 { get; set; }
        public virtual Topic Topic2 { get; set; }
        public virtual ICollection<WebinarTopicXref> WebinarTopicXrefs { get; set; }
        public State DomainEntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
