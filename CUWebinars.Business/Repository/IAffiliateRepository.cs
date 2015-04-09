using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IAffiliateRepository: IDisposable
    {
        Affiliate AttachItem(Affiliate item);
        bool Exists(Affiliate item);
        Affiliate FindByIdAndDetachItem(int id);
        Affiliate FindByIdWithIncluding(int id, params Expression<Func<Affiliate, object>>[] includeProperties);
        IQueryable<Affiliate> GetAffiliates();
        IEnumerable<Affiliate> GetAll();
        Affiliate GetCurrentAffiliate();
        Affiliate FindById(int id);
        Affiliate LoadByTTSDomain(string ttsDomain);
        IQueryable<Order> GetOrdersByUser(int affiliateId, int userId);
        IQueryable<Order> GetOrders(int affiliateId);

    }
}
