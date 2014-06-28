using CUWebinars.Business.AccountService;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Services;
using System;

namespace CUWebinars.Business.CQS.QueryHandlers
{
    public class OrderManagementQueryHandlers : 
        IQueryHandler<OrderManagementQuery, OrderManagementQueryResult>
    {
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public OrderManagementQueryHandlers(IMembershipService membershipService, IOrderManagementService orderManagementService)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
        }


        public OrderManagementQueryResult Handle(OrderManagementQuery query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var orderManagementQueryResult = new OrderManagementQueryResult
            {
                Affiliate = _orderManagementService.GetAffiliateById(query.AffiliateId),
                WebUser = _membershipService.GetUserByEmail(query.Email),
                Webinar = _orderManagementService.GetWebinar(query.WebinarId)
            };

            return orderManagementQueryResult;
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
