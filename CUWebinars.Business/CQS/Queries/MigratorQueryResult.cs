using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Queries
{
    public class MigratorQueryResult
    {
        public Affiliate Affiliate { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
        public int LegacyUserId { get; set; }
        public int LegacyOrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
