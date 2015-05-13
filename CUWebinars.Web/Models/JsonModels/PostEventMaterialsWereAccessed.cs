using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Models.JsonModels
{
    public class PostEventMaterialsWereAccessed
    {
 
        public String OnDemandCode{ get; set; }
        public DateTime DateAccessed { get; set; }
        public String UserName { get; set; }
        public String UserEmail { get; set; }
        public String UserIP { get; set; }
    }
}