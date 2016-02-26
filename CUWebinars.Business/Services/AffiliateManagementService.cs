using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System.Net;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using CUWebinars.Web.Models;
using FluentValidation;
using Ninject.Extensions.Logging;

namespace CUWebinars.Business.Services
{
    public class AffiliateManagementService : IEventSource, IAffiliateManagementService
    {
        private readonly ILogger _logger;
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IWebinarRepository _webinarRepository;
        //private readonly IOrderRepository _orderRepository;
        
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public AffiliateManagementService(ILogger logger, IOrderRepository orderRepository, IWebinarRepository webinarRepository,IAffiliateRepository affiliateRepository )
        {
            _affiliateRepository = affiliateRepository;
            _orderRepository = orderRepository;
            _webinarRepository = webinarRepository;
            _logger = logger;
        }

        private void CalculateAffiliateRevenuesAndComissions(IList<AffiliateReportDTO> reportData)
        {
            //Calculate total revenues
            foreach (AffiliateReportDTO registration in reportData)
            {
                foreach (Order order in registration.Orders)
                {
                    if (order.OrderStatus == OrderStatus.Submitted
                        || order.OrderStatus == OrderStatus.Billed
                        || order.OrderStatus == OrderStatus.Paid)
                    {
                        registration.TotalRevenues += order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).RowPrice;
                    }
                }
            }

            //Calculate total commissions

            //NoCommission = 0,
            //Sliding4TierNoCCBreak =1,
            //Sliding4TierPlusCC30 = 5, 
            //Flat35plusCC30 = 2,
            //Flat40 = 3,
            //Flat35 = 4

            foreach (AffiliateReportDTO registration in reportData)
            {
                switch (registration.Affiliate.CommissionModel)
                {
                    case 1: // Sliding4TierNoCCBreak:
                        int numberOfRegistrations = 0;
                        foreach (Order order in registration.Orders)
                        {
                            OrderRow row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                            decimal commissionPercent = 0.0M;
                            numberOfRegistrations++;
                            if (numberOfRegistrations >= 16)
                            {
                                commissionPercent = 0.45M;
                            }
                            else if (numberOfRegistrations >= 11)
                            {
                                commissionPercent = 0.4M;
                            }
                            else if (numberOfRegistrations >= 6)
                            {
                                commissionPercent = 0.35M;
                            }
                            else //nonCreditCardPayments < 6
                            {
                                commissionPercent = 0.3M;
                            }

                            //if (row.Webinar.IsSubscriptionWebinar)
                            //{
                            //    commissionPercent = 0.35M;
                            //}
                            row.Royalty = row.RowPrice * commissionPercent;
                            registration.TotalCommissions += row.Royalty;
                        }
                        break;


                    case 3:// CommissionModel.Flat40:

                        foreach (Order order in registration.Orders)
                        {
                            OrderRow row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                            decimal commissionPercent = 0.0M;
                            //if (row.Webinar.IsSubscriptionWebinar)
                            //{
                            //    commissionPercent = 0.35M;
                            //}
                            //else
                            //{
                            commissionPercent = 0.4M;
                            //}

                            row.Royalty = row.RowPrice * commissionPercent;
                            registration.TotalCommissions += row.Royalty;
                        }
                        break;
                    case 4:// CommissionModel.Flat35:
                        foreach (Order order in registration.Orders)
                        {
                            OrderRow row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                            decimal commissionPercent = 0.35M;
                            //if (row.Order.PaymentType == PaymentType.CreditCard ||
                            //    row.Webinar.IsSubscriptionWebinar)
                            //{
                            //    commissionPercent = 0.35M;
                            //}
                            //else
                            //{
                            //    commissionPercent = 0.4M;
                            //}

                            row.Royalty = row.RowPrice * commissionPercent;
                            registration.TotalCommissions += row.Royalty;
                        }
                        break;
                    case 0:// CommissionModel.NoCommission:
                        registration.TotalCommissions = 0;
                        break;


                    default:
                        throw new TTSException("Invalid commission model " +
                            registration.Affiliate.CommissionModel);

                    //not currently in use:
                    //case 2:// CommissionModel.Flat35plusCC30:
                    //    foreach (OrderRow row in registration.OrderRows)
                    //    {
                    //        decimal commissionPercent = 0.0M;
                    //        if (row.Order.PaymentType == PaymentType.CreditCard)
                    //        {
                    //            commissionPercent = 0.3M;
                    //        }
                    //        else
                    //        {
                    //            commissionPercent = 0.35M;
                    //        }

                    //        row.Royalty = row.RowPrice * commissionPercent;
                    //        registration.TotalCommissions += row.Royalty;
                    //    }
                    //    break;
                    //case 5:// CommissionModel.Sliding4TierPlusCC30:
                    //    int nonCreditCardPayments = 0;
                    //    foreach (OrderRow row in registration.OrderRows)
                    //    {
                    //        decimal commissionPercent = 0.0M;
                    //        if (row.Order.PaymentType == 0)
                    //        {
                    //            commissionPercent = 0.3M;
                    //        }
                    //        else
                    //        {
                    //            nonCreditCardPayments++;
                    //            if (nonCreditCardPayments >= 16)
                    //            {
                    //                commissionPercent = 0.45M;
                    //            }
                    //            else if (nonCreditCardPayments >= 11)
                    //            {
                    //                commissionPercent = 0.4M;
                    //            }
                    //            else if (nonCreditCardPayments >= 6)
                    //            {
                    //                commissionPercent = 0.35M;
                    //            }
                    //            else //nonCreditCardPayments < 6
                    //            {
                    //                commissionPercent = 0.3M;
                    //            }
                    //        }
                    //        //if (row.Webinar.IsSubscriptionWebinar)
                    //        //{
                    //        //    commissionPercent = 0.35M;
                    //        //}
                    //        row.Royalty = row.RowPrice * commissionPercent;
                    //        registration.TotalCommissions += row.Royalty;
                    //    }
                    //    break;

                }
            }
        }
        public virtual IList<AffiliateReportDTO> AffiliateReport(int webinarID)
        {
            IList<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID).ToList();

            IList<AffiliateReportDTO> reportData = BuildAffiliateReport(orders, webinarID);
            return reportData;
        }


        //public virtual AffiliateInvoiceDTO AffiliateInvoice(int webinarID, int affiliateID)
        //{
        //    IList<OrderRow> orderRows = OrderFacade.Instance.LoadSubmittedOrderRowsPerWebinar(webinarID,
        //        affiliateID);

        //    IList<AffiliateInvoiceDTO> reportData = BuildAffiliateInvoice(orderRows, webinarID);
        //    if (reportData.Count == 0)
        //    {
        //        return new AffiliateInvoiceDTO { WebinarID = webinarID, Affiliate = AffiliateFacade.Instance.Load(affiliateID) };
        //    }

        //    return reportData[0];
        //}

        public virtual AffiliateReportDTO AffiliateReport(int webinarID, int affiliateID)
        {
            IList<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID).ToList();

            IList<AffiliateReportDTO> reportData = BuildAffiliateReport(orders, webinarID);
            if (reportData.Count == 0)
            {
                return new AffiliateReportDTO { WebinarID = webinarID, Affiliate = FindById(affiliateID) };
            }

            return reportData[0];
        }

        private IList<AffiliateReportDTO> BuildAffiliateReport(IList<Order> orders, int webinarID)
        {
            var registrations =
                from o in orders
                where o.Affiliate != null
                orderby o.idOrder ascending
                group o by o.Affiliate into u
                select new AffiliateReportDTO
                {
                    WebinarID = webinarID,
                    Affiliate = u.Key,
                    Orders = u.ToList<Order>(),
                };

            IList<AffiliateReportDTO> reportData = registrations.ToList<AffiliateReportDTO>();

            CalculateAffiliateRevenuesAndComissions(reportData);

            return reportData;
        }
        //private IList<AffiliateInvoiceDTO> BuildAffiliateInvoice(IList<OrderRow> orderRows, int webinarID)
        //{
        //    var registrations =
        //        from r in orderRows
        //        where r.Order.Affiliate != null
        //        orderby r.Order.ID ascending
        //        group r by r.Order.Affiliate into u
        //        select new AffiliateInvoiceDTO
        //        {
        //            WebinarID = webinarID,
        //            Affiliate = u.Key,
        //            OrderRows = u.ToList<OrderRow>(),
        //        };

        //    IList<AffiliateInvoiceDTO> reportData = registrations.ToList<AffiliateInvoiceDTO>();

        //    CalculateAffiliateRoyalties(reportData);

        //    return reportData;
        //}

        //private void CalculateAffiliateRoyalties(IList<AffiliateInvoiceDTO> reportData)
        //{
        //    //Calculate total revenues
        //    foreach (AffiliateInvoiceDTO registration in reportData)
        //    {
        //        foreach (OrderRow row in registration.OrderRows)
        //        {
        //            if (registration.Affiliate.BillingModel == "TTS")
        //            {
        //                if (row.Status == OrderRowStatus.Submitted || row.Status == OrderRowStatus.Billed)
        //                {
        //                    registration.TotalOnBilled += row.RowPrice;

        //                }
        //                if (row.Status == OrderRowStatus.Paid)
        //                {
        //                    registration.TotalOnPaid += row.RowPrice;
        //                }
        //            }

        //            if (row.Status == OrderRowStatus.Submitted || row.Status == OrderRowStatus.Billed ||
        //                row.Status == OrderRowStatus.Paid)
        //            {
        //                registration.TotalOnPaid += row.RowPrice;
        //            }
        //        }
        //    }

        //    //Calculate total commissions
        //    foreach (AffiliateInvoiceDTO registration in reportData)
        //    {
        //        var order = OrderFacade.Instance.LoadOrderByOrderRowID(registration.OrderRows.SingleOrDefault().ID);

        //    }
        //}
            
            
        

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
            return _affiliateRepository.LoadById(id);
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
