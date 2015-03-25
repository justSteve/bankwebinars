
using System;

namespace CUWebinars.Business.CQS.Queries
{
    public class ImportQuery : IQuery<ImportQueryResult>
    {
        public int AffiliateId { get; set; }
        public string Email { get; set; }
        public int WebinarId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
