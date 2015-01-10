
using System;

namespace CUWebinars.Business.CQS.Queries
{
    public class MigratorQuery : IQuery<MigratorQueryResult>
    {
        public int AffiliateId { get; set; }
        public string Email { get; set; }
        public int WebinarId { get; set; }
        public int LegacyUserId { get; set; }
        public int LegacyOrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
