using System;
using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories
{
    public class AffiliateRepository: IAffiliateRepository

    {        TTSWebinarsContext _ctx;

        public AffiliateRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }
        public IQueryable<Affiliate> GetAffiliates()
        {
            return _ctx.Affiliates.Where(u => u.WebUser.UserType == UserType.Affiliate);
        }        
        public Affiliate GetCurrentAffiliate()
        {
            return _ctx.Affiliates.Single(a => a.WebUser.email == "affiliate@ttstrain.com");
        }   

        public IQueryable<Order> GetOrdersByUser(int affiliateId, int userId)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Order> GetOrders(int affiliateId)
        {
            throw new NotImplementedException();
        }
    }
}