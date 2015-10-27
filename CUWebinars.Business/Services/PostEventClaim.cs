using System;

namespace CUWebinars.Business.Services
{
    public class PostEventClaim
    {
        public int OrderId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OnDemandCode { get; set; }
    }
}