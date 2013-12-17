using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories.Interfaces
{
    public interface IAffiliateRepository 
    {
        IQueryable<Affiliate> GetAffiliates();
        Affiliate GetCurrentAffiliate();
        IQueryable<Order> GetOrdersByUser(int affiliateId, int userId);
        IQueryable<Order> GetOrders(int affiliateId);

    }
}
