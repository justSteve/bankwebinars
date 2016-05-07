using System;
using System.Collections;
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

        private readonly TTSWebinarsContext _context;
        //private readonly IOrderRepository _orderRepository;

        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public AffiliateManagementService(ILogger logger, IOrderRepository orderRepository, IWebinarRepository webinarRepository, IAffiliateRepository affiliateRepository, TTSWebinarsContext context)
        {
            _affiliateRepository = affiliateRepository;
            _orderRepository = orderRepository;
            _webinarRepository = webinarRepository;
            _logger = logger;
            _context = context;
        }

        private void ComputeRoyalty(IList<Order> orders)
        {
            //Calculate total revenues
            // saves that calculation to Row.Royalty

            //NoCommission = 0,
            //Sliding4TierNoCCBreak =1,
            //Sliding4TierPlusCC30 = 5, 
            //Flat35plusCC30 = 2,
            //Flat40 = 3,
            //Flat35 = 4

            //Calculate total commissions
            foreach (var registration in orders)
            {
                switch (registration.Affiliate.CommissionModel)
                {
                    case 1: // Sliding4TierNoCCBreak:
                        int numberOfRegistrations = 0;
                        foreach (Order order in orders.OrderBy(o => o.OrderDate))
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
                            row.Royalty = row.RowPrice * commissionPercent;
                            //registration.TotalCommissions += row.Royalty;
                        }
                        break;

                    case 3:// CommissionModel.Flat40:
                        foreach (Order order in orders.OrderBy(o => o.OrderDate))
                        {
                            OrderRow row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                            decimal commissionPercent = 0.0M;

                            commissionPercent = 0.4M;

                            row.Royalty = row.RowPrice * commissionPercent;
                            //registration.TotalCommissions += row.Royalty;
                        }
                        break;
                    case 4:// CommissionModel.Flat35:
                        foreach (Order order in orders.OrderBy(o => o.OrderDate))
                        {
                            OrderRow row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                            decimal commissionPercent = 0.35M;

                            row.Royalty = row.RowPrice * commissionPercent;
                            //registration.TotalCommissions += row.Royalty;
                        }
                        break;

                    default:
                        throw new TTSException("Invalid commission model " +
                            registration.Affiliate.CommissionModel);

                    //:not currently in use
                    #region not currently in use
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
                    #endregion

                }
            }

            _orderRepository.SaveChanges();
        }

        public virtual IList<AffiliateReportDTO> AffiliateReport(int webinarID)
        {
            List<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID).ToList();

            IList<AffiliateReportDTO> reportData = BuildAffiliateReport(orders, webinarID);
            return reportData;
        }


        public virtual AffiliateInvoiceDTO AffiliateInvoice(int webinarID, int affiliateID)
        {
            List<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID)
                .Where(o => o.idAffiliate == affiliateID).ToList();


            IList<AffiliateInvoiceDTO> reportData = BuildAffiliateInvoice(orders, webinarID);
            if (reportData.Count == 0)
            {
                return new AffiliateInvoiceDTO { WebinarID = webinarID, Affiliate = FindById(affiliateID) };
            }

            return reportData[0];
        }

        public virtual AffiliateReportDTO AffiliateReport(int webinarID, int affiliateID)
        {
            List<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID).ToList();

            IList<AffiliateReportDTO> reportData = BuildAffiliateReport(orders, webinarID);
            if (reportData.Count == 0)
            {
                return new AffiliateReportDTO { WebinarId = webinarID, Affiliate = FindById(affiliateID) };
            }

            return reportData[0];
        }

        public IList<AffiliateReportDTO> BuildAffiliateReport(List<Order> orders, int webinarID)
        {
            try
            {
                var listofAffiliates = orders.OrderBy(o => o.OrderDate)
                       .GroupBy(o => o.Affiliate)
                       .Select(o => o.Key).ToList();

                foreach (var affiliate in listofAffiliates)
                {

                    BuildAffiliateInvoice(orders.Where(o => o.idAffiliate == affiliate.idUserAff
                        && (o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Submitted))
                        .ToList(), webinarID);
                }


            }
            catch (Exception)
            {

                throw;
            }
            return orders.Where(o => o.Affiliate != null)
                    .OrderBy(o => o.OrderDate)
                    .GroupBy(o => o.Affiliate)
                    .Select(u => new AffiliateReportDTO
                    {
                        WebinarId = webinarID,
                        Affiliate = u.Key,
                        Orders = u.ToList(),
                    }).ToList();

        }



        public IList<AffiliateInvoiceDTO> BuildAffiliateInvoice(List<Order> orders, int webinarID)
        {
            ComputeRoyalty(orders);

            IList<AffiliateInvoiceDTO> reportData = orders.Where(o => o.Affiliate != null)
                    .OrderBy(o => o.idOrder)
                    .GroupBy(o => o.Affiliate)
                    .Select(u => new AffiliateInvoiceDTO()
                    {
                        WebinarID = webinarID,
                        Affiliate = u.Key,
                        Orders = u.ToList<Order>(),
                    }).ToList();


            return reportData;
        }

        public AffiliateInvoiceDTO GetAffiliateInvoice(int idWebinar, string aff)
        {

            decimal _totalDue = 0;
            decimal _totalOnBilled = 0;
            decimal _totalDiscounts = 0;
            decimal _totalOnPaid = 0;
            decimal _totalRoyalties = 0;
            int idAffiliate = Convert.ToInt32(aff);
            Webinar webinar = _webinarRepository.FindById(idWebinar);
            var orders = _orderRepository.GetOrdersByWebinar(idWebinar).Where(a => a.idAffiliate == idAffiliate).ToList();
            IList<OrderRowModel> rows = new List<OrderRowModel>();

            ComputeRoyalty(orders);

            var ordinalHolder = 0;
            foreach (var order in orders)
            {
                ordinalHolder++;
                var orow = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                OrderRowModel row = new OrderRowModel
                {
                    Royalty = orow.Royalty.ToString(),
                    Email = order.BillingEmail,
                    Institution = order.Institution,
                    Name = order.FirstName + " " + order.LastName,
                    OrderID = order.idOrder.ToString(),
                    Percent = GetRowPercent(ordinalHolder, FindById(idAffiliate).CommissionModel),
                    Price = order.Total.ToString(),
                    Discount = (orow.UnitPrice - orow.RowPrice).ToString(),
                    Status = order.OrderStatus.ToString()
                };

                if (order.OrderStatus == OrderStatus.Paid)
                {
                    _totalOnPaid += order.Total;
                }
                if (order.OrderStatus == OrderStatus.Submitted)
                {
                    _totalOnBilled += order.Total;
                }
                _totalDue += order.Total;
                _totalRoyalties += orow.Royalty;

                rows.Add(row);


            }

            var model = new AffiliateInvoiceDTO
            {
                Affiliate = _affiliateRepository.FindById(idAffiliate),
                AffiliateName = _affiliateRepository.FindById(idAffiliate).DisplayTitle,
                InoviceDate = DateTime.Now.ToShortDateString(),
                InvoiceId = "Invoice #: " + idAffiliate + '-' + idWebinar,
                WebinarTitle = webinar.Title,
                WebinarDate = webinar.Date.ToShortDateString(),
                WebinarID = webinar.idWebinar,
                Orders = orders,
                Rows = rows,
                TotalDiscounts = _totalDiscounts,
                TotalNetDue = _totalDue,
                TotalOnBilled = _totalOnBilled,
                TotalOnPaid = _totalOnPaid,
                TotalRoyalties = _totalRoyalties

            };
            return model;
        }

        public IList<DiscountDTO> GetSubscriptionsByAffiliate(int idUserAff)
        {
            IList<DiscountDTO> theseSubscriptions = new List<DiscountDTO>();

            if (idUserAff != 19)
            {
                var theseDiscounts = _context.Discounts
                    .Where(d => d.DiscountType == DiscountType.Subscription && d.idAffiliate == idUserAff).ToList();

                foreach (var discount in theseDiscounts)
                {
                    var thisSub = new DiscountDTO
                    {
                        idAffiliate = idUserAff,
                        idDiscount = discount.idDiscount,
                        DateBilled = discount.DateBilled,
                        DateValidFrom = discount.DateValidFrom,
                        DateValidTo = discount.DateValidTo,
                        DiscountCode = discount.DiscountCode,
                        Notes = discount.Notes,
                        RenewalTerm = discount.RenewalTerm,
                        Status = discount.Status
                    };
                    theseSubscriptions.Add(thisSub);
                }
            }
            else
            {
                var theseDiscounts = _context.Discounts
                    .Where(d => d.DiscountType == DiscountType.Subscription).ToList();

                foreach (var discount in theseDiscounts)
                {
                    var thisSub = new DiscountDTO
                    {
                        idAffiliate = idUserAff,
                        idDiscount = discount.idDiscount,
                        DateBilled = discount.DateBilled,
                        DateValidFrom = discount.DateValidFrom,
                        DateValidTo = discount.DateValidTo,
                        DiscountCode = discount.DiscountCode,
                        Notes = discount.Notes,
                        RenewalTerm = discount.RenewalTerm,
                        Status = discount.Status
                    };
                    theseSubscriptions.Add(thisSub);
                }
            }

            return theseSubscriptions;
        }

        private string GetRowPercent(int ordinalHolder, byte commissionModel)
        {
            return "30%";
        }

        private void CalculateAffiliateRoyalties(IList<AffiliateInvoiceDTO> reportData)
        {
            //Calculate total revenues
            foreach (AffiliateInvoiceDTO registration in reportData)
            {
                foreach (var order in registration.Orders)
                {
                    var row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                    if (registration.Affiliate.BillingModel == "TTS")
                    {
                        if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed)
                        {
                            registration.TotalOnBilled += row.RowPrice;

                        }
                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            registration.TotalOnPaid += row.RowPrice;
                        }
                    }

                    if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed ||
                        order.OrderStatus == OrderStatus.Paid)
                    {
                        registration.TotalOnPaid += row.RowPrice;
                    }
                }
            }

            //Calculate total commissions
            //foreach (AffiliateInvoiceDTO registration in reportData)
            //{
            //    var order = _orderRepository.GetOrderById(registration.id);

            //}
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
