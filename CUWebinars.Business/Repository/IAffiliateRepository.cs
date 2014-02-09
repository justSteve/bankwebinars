using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IAffiliateRepository 
    {
        IQueryable<Affiliate> GetAffiliates();
        Affiliate GetCurrentAffiliate();
        Affiliate FindById(int id);
        Affiliate LoadByTTSDomain(string ttsDomain);
        IQueryable<Order> GetOrdersByUser(int affiliateId, int userId);
        IQueryable<Order> GetOrders(int affiliateId);

    }
}
