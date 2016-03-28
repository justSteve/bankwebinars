using System.Linq;
using System.Linq.Expressions;
using CUWebinars.Business.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using CUWebinars.Web.Models;


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

        IList<AffiliateReportDTO> BuildAffiliateReport(List<Order> orders, int webinarId);
        IList<AffiliateInvoiceDTO> BuildAffiliateInvoice(List<Order> orders, int webinarId);
        AffiliateInvoiceDTO GetAffiliateInvoice(int value, string aff);
        IList<DiscountDTO> GetSubscriptionsByAffiliate(int idUserAff);
    }
}
