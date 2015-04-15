using System.Linq;
using System.Linq.Expressions;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IAffiliateManagementService : IDisposable
    {
        //Affiliate AttachItem(Affiliate item);
        //bool Exists(Affiliate item);
        //Affiliate FindByIdAndDetachItem(int id);
        //Affiliate FindByIdWithIncluding(int id, params Expression<Func<Affiliate, object>>[] includeProperties);
        IQueryable<Affiliate> GetAffiliates();
        //Affiliate GetCurrentAffiliate();
        Affiliate FindById(int id);
        Affiliate LoadByTTSDomain(string ttsDomain);
        IQueryable<Order> GetOrdersByUser(int affiliateId, int userId);
        IQueryable<Order> GetOrders(int affiliateId);
      
    }
}
