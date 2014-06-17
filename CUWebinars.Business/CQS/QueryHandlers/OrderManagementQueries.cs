using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;

namespace CUWebinars.Business.CQS.QueryHandlers
{
    public class OrderManagementQueries : 
        IQueryHandler<OrderManagementQuery, OrderManagementQueryResult>
    {
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;

        public OrderManagementQueries(IMembershipService membershipService, IOrderManagementService orderManagementService)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
        }


        public OrderManagementQueryResult Handle(OrderManagementQuery query)
        {
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
            throw new NotImplementedException();
        }
    }
}
