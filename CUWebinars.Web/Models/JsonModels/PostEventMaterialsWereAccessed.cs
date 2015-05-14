using System;

namespace CUWebinars.Web.Models.JsonModels
{
    public class PostEventMaterialsWereAccessed
    {
 
        public string OnDemandCode{ get; set; }
        public DateTime DateAccessed { get; set; }
        public String UserName { get; set; }
        public String UserEmail { get; set; }
        public String UserAudit { get; set; }
    }
}