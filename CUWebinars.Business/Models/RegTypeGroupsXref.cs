
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class RegTypesGroupsXref : IObjectWithState
    {
        public int idWebinarRegTypeGroup { get; set; }
        public int idWebinar { get; set; }
        public int idRegTypeGroup { get; set; }
        public virtual RegTypesGroup RegTypesGroup { get; set; }
        public virtual Webinar Webinar { get; set; }
        public State DomainEntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
