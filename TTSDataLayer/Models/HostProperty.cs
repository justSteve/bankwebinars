using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class HostProperty
    {
        public HostProperty()
        {
            //this.HostPropertyValues = new List<HostPropertyValue>();
        }

        public int idHostProperty { get; set; }
        //public int idHost { get; set; }
        public string name { get; set; }
        //public virtual ICollection<HostPropertyValue> HostPropertyValues { get; set; }
    }
}
