using System;
using System.Linq.Expressions;
using CUWebinars.Business.Models;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class AffiliateRepository : TTSWebinarsRepository<TTSWebinarsContext, Affiliate>, IAffiliateRepository
    {
        public AffiliateRepository()
        {
     
        }

        public AffiliateRepository(TTSWebinarsContext context)
            : base(context)
        {
            
        }

        
        public Affiliate FindByIdWithIncluding(int id, params Expression<Func<Affiliate, object>>[] includeProperties)
        {
            IQueryable<Affiliate> queryable = items;
            foreach (Expression<Func<Affiliate, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<Affiliate, object>(includeProperty);
            }

                return queryable.First(a => a.idUserAff == id);
        }

        public IQueryable<Affiliate> GetAffiliates()
        {
            return items.Where(a => a.WebUser.UserType == UserType.Affiliate);
        }

        public Affiliate GetCurrentAffiliate()
        {
            return items.Single(a => a.WebUser.email == "Mark_Bennett@ttstrain.com"); 
            //hardwired for CUWebinars
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
