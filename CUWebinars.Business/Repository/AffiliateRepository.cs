using CUWebinars.Business.Models;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class AffiliateRepository : TTSWebinarsRepository<TTSWebinarsContext, Affiliate>, IAffiliateRepository
    {
        public RefDataRepository RefContext { get; set; }

        public AffiliateRepository()
        {
            RefContext = new RefDataRepository();
        }

        public AffiliateRepository(TTSWebinarsContext context)
            : base(context)
        {
            
        }

        public IQueryable<Affiliate> GetAffiliates()
        {
            return items.Where(a => a.WebUser.UserType == UserType.Affiliate);
        }

        public Affiliate GetCurrentAffiliate()
        {
            return items.Single(a => a.WebUser.email == "Mark_Bennett@ttstrain.com"); // todo: hardwired. Is this required?
        }

        public Affiliate LoadByTTSDomain(string ttsDomain)
        {
            return items.Single(a => a.ttsDomain == ttsDomain);
        }

        public IQueryable<Order> GetOrdersByUser(int affiliateId, int userId)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<Order> GetOrders(int affiliateId)
        {
            throw new System.NotImplementedException();
        }
    }
}
