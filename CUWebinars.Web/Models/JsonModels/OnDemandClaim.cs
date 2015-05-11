using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace CUWebinars.Web.Models.JsonModels
{

    public class OnDemandClaim
    {
        //"{\"OrderId\":43760,\"\":\"2015-11-05\",\"ObfuscationString\":\"8YLGW\"}"
        public int OrderId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ObfuscationString { get; set; }
    }
}