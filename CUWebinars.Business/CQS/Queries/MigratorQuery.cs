
namespace CUWebinars.Business.CQS.Queries
{
    public class MirgratorQuery : IQuery<MigratorQueryResult>
    {
        public int AffiliateId { get; set; }
        public string Email { get; set; }
        public int WebinarId { get; set; }
    }
}
