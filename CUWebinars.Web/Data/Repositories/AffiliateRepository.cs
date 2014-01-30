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
            return _ctx.Affiliates.Single(a => a.WebUser.email == "Mark_Bennett@ttstrain.com");
        }           
        
        public Affiliate SetCurrentAffiliate(int ID)
        {
            return _ctx.Affiliates.Single(a => a.idUserAff == ID);
        }

        public Affiliate LoadByTTSDomain(string ttsDomain)
        {
            return _ctx.Affiliates.Single(a => a.ttsDomain == ttsDomain);
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