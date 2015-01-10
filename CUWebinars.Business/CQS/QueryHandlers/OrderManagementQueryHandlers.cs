using CUWebinars.Business.AccountService;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using System;

namespace CUWebinars.Business.CQS.QueryHandlers
{
    public class OrderManagementQueryHandlers :
        IQueryHandler<OrderManagementQuery, OrderManagementQueryResult>,
IQueryHandler<MigratorQuery, MigratorQueryResult>, IQueryHandler<ImportQuery, ImportQueryResult>
    {
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private bool _disposed;

        public OrderManagementQueryHandlers(IMembershipService membershipService, IOrderManagementService orderManagementService, IWebinarManagementService webinarManagementService)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
        }


        public OrderManagementQueryResult Handle(OrderManagementQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var orderManagementQueryResult = new OrderManagementQueryResult
            {
                Affiliate = _orderManagementService.GetAffiliateById(query.AffiliateId),
                WebUser = _membershipService.GetUserByEmail(query.Email),
                Webinar = _webinarManagementService.GetWebinar(query.WebinarId)
            };

            return orderManagementQueryResult;
        }

        public MigratorQueryResult Handle(MigratorQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");
            //TODO: Why is affiliate not being instantiated here.
            var migrateQueryResult = new MigratorQueryResult
            {
                Affiliate = _orderManagementService.GetAffiliateByIdLoaded(query.AffiliateId),
                //Affiliate = _orderManagementService.GetAffiliateById(query.AffiliateId),
                WebUser = _membershipService.GetUserByEmail(query.Email),
                LegacyOrderId = query.LegacyOrderId,
                LegacyUserId = query.LegacyUserId,
                Webinar = _webinarManagementService.GetWebinar(query.WebinarId)
            };

            return migrateQueryResult;
        }
        public ImportQueryResult Handle(ImportQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var importQueryResult = new ImportQueryResult
            {
                Affiliate = _orderManagementService.GetAffiliateById(query.AffiliateId),
                WebUser = _membershipService.GetUserByEmail(query.Email),
                Webinar = _webinarManagementService.GetWebinar(query.WebinarId)
            };

            return importQueryResult;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _membershipService.Dispose();
                _orderManagementService.Dispose();

            }
            _disposed = true;
        }
    }
}
