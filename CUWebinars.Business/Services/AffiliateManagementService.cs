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
        private readonly IAffiliateRepository _affiliateRepository;
        //private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

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
                _webUserRepository.Dispose();

                _disposed = true;
            }
        }


        public IQueryable<Affiliate> GetAffiliates()
        {
            throw new NotImplementedException();
        }

        public Affiliate GetCurrentAffiliate()
        {
            throw new NotImplementedException();
        }

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
