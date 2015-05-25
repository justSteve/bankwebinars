using System;

namespace CUWebinars.Web.Models.JsonModels
{

    public class OnDemandClaim
    {
        //"{\"OrderId\":43760,\"\":\"2015-11-05\",\"OnDemandCode\":\"8YLGW\"}"
        public int OrderId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OnDemandCode { get; set; }
    }
}