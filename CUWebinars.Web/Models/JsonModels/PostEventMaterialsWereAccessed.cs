using System;

namespace CUWebinars.Web.Models.JsonModels
{
    public class PostEventMaterialsWereAccessed
    {
 
        public string OnDemandCode{ get; set; }
        public DateTimeOffset DateAccessed { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserIP { get; set; }
    }
}