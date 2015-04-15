using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System.Net;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using FluentValidation;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Services
{
    public class AffiliateManagementService : IEventSource, IAffiliateManagementService
    {
        private readonly ILogger _logger;
        private readonly IAffiliateRepository _affiliateRepository;
        //private readonly IOrderRepository _orderRepository;
        
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public AffiliateManagementService(ILogger logger, IAffiliateRepository affiliateRepository )
        {
            _affiliateRepository = affiliateRepository;
            _logger = logger;
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return _events;
        }

        public void Clear()
        {
            _events.Clear();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _affiliateRepository.Dispose();
                
                _disposed = true;
            }
        }


        public IQueryable<Affiliate> GetAffiliates()
        {
            return _affiliateRepository.GetAffiliates();
        }

        //public Affiliate GetCurrentAffiliate()
        //{
        //    return _affiliateRepository.GetCurrentAffiliate();
        //}

        public Affiliate FindById(int id)
        {
            throw new NotImplementedException();
        }

        public Affiliate LoadByTTSDomain(string ttsDomain)
        {
            return _affiliateRepository.LoadByTTSDomain(ttsDomain);
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
