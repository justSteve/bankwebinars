
namespace CUWebinars.Business.CQS.Queries
{
    public class OrderManagementQuery : IQuery<OrderManagementQueryResult>
    {
        public int AffiliateId { get; set; }
        public string Email { get; set; }
        public int WebinarId { get; set; }
    }
}
