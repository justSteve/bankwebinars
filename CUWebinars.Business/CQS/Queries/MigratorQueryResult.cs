using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Queries
{
    public class MigratorQueryResult
    {
        public Affiliate Affiliate { get; set; }
        public Webinar Webinar { get; set; }
        public WebUser WebUser { get; set; }
    }
}
