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

        private AffiliateInvoiceDTO ComputeRoyalty(AffiliateInvoiceDTO invoice, IList<Order> orders, int idAffiliate, bool generatingWeeklyReport)
        {

            invoice.Affiliate = FindById(idAffiliate);
            decimal totalBilledRevenue = 0M;
            decimal totalBilledRoyalty = 0M;
            decimal totalPaidRoyalty = 0M;

            switch (invoice.Affiliate.CommissionModel)
            {
                case 1: // Sliding4TierNoCCBreak:
                    int numberOfRegistrations = 0;

                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        numberOfRegistrations++;
                        decimal commissionPercent;

                        var row = IniInvoice(order, invoice);

                        if (numberOfRegistrations < 6)
                        {
                            commissionPercent = 0.3M;
                        }
                        else if (numberOfRegistrations < 11)
                        {
                            commissionPercent = 0.35M;
                        }
                        else if (numberOfRegistrations < 16)
                        {
                            commissionPercent = 0.4M;
                        }
                        else //
                        {
                            commissionPercent = 0.45M;
                        }

                        row.Royalty = row.RowPrice * commissionPercent;

                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            totalPaidRoyalty += row.Royalty;
                        }
                        else
                        {
                            totalBilledRevenue += row.RowPrice;
                            totalBilledRoyalty += row.Royalty;
                        }
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties += row.Royalty;
                    }
                    break;

                case 3: // CommissionModel.Flat40

                    numberOfRegistrations = 0;

                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        decimal commissionPercent;
                        var row = IniInvoice(order, invoice);
                        commissionPercent = 0.4M;

                        //row.Royalty = order.Total * commissionPercent;
                        row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;


                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            totalPaidRoyalty += row.Royalty;
                        }
                        else
                        {
                            totalBilledRevenue += row.RowPrice;
                            totalBilledRoyalty += row.Royalty;
                        }
                    }
                    break;
                case 4: // CommissionModel.Flat35:
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {

                        var row = IniInvoice(order, invoice);
                        decimal commissionPercent = 0.35M;

                        //row.Royalty = order.Total * commissionPercent;
                        row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;


                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            totalPaidRoyalty += row.Royalty;
                        }
                        else
                        {
                            totalBilledRevenue += row.RowPrice;
                            totalBilledRoyalty += row.Royalty;
                        }

                    }
                    break;
                case 5: // CommissionModel.Flat25


                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        decimal commissionPercent = 0.25M;

                        var row = IniInvoice(order, invoice);
                        //row.Royalty = order.Total * commissionPercent;
                        row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;


                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            totalPaidRoyalty += row.Royalty;
                        }
                        else
                        {
                            totalBilledRevenue += row.RowPrice;
                            totalBilledRoyalty += row.Royalty;
                        }
                    }
                    break;

                default:
                    throw new TTSException("Invalid commission model ");
            }

            if (invoice.Affiliate.BillingModel.Trim(' ') == "aff")
            {
                invoice.TotalNetDue = (invoice.TotalOnBilled + invoice.TotalOnPaid - invoice.TotalDiscounts) - invoice.TotalRoyalties;
            }
            if (invoice.Affiliate.BillingModel.Trim(' ') == "billed")
            {
                invoice.TotalNetDue = totalBilledRevenue - totalBilledRoyalty - totalPaidRoyalty;
            }

            return invoice;


        }

        private OrderRow IniInvoice(Order order, AffiliateInvoiceDTO invoice)
        {

            OrderRow row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            if (row == null) throw new ArgumentNullException("row");
            if (order.OrderStatus == OrderStatus.Paid)
            {
                invoice.TotalOnPaid += row.RowPrice;
            }
            if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed)
            {
                invoice.TotalOnBilled += row.RowPrice;
                _logger.Info("TotalOnBilled =  " + invoice.TotalOnBilled);
                order.OrderStatus = OrderStatus.Billed;
            }

            if (row.Discount != null)
            {
                if (row.Discount.PercentOff > 0)
                {
                    invoice.TotalDiscounts = invoice.TotalDiscounts + (row.UnitPrice * ((row.Discount.PercentOff) / 100));
                    _logger.Info(invoice.Affiliate.idUserAff + "-" + row.idOrder + "-" + (row.UnitPrice * ((row.Discount.PercentOff) / 100)));

                }
                if (row.Discount.FlatOff > 0)
                {
                    invoice.TotalDiscounts = invoice.TotalDiscounts + row.UnitPrice - row.Discount.FlatOff;
                    _logger.Info(invoice.Affiliate.idUserAff + "-" + row.idOrder + "-" + (row.UnitPrice * ((row.Discount.PercentOff) / 100)));
                }
            }
            return row;
        }


        //public virtual IList<AffiliateReportDTO> AffiliateReport(int webinarID)
        //{
        //    List<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID).ToList();

        //    IList<AffiliateReportDTO> reportData = BuildAffiliateReport(orders, webinarID);
        //    return reportData;
        //}


        //public virtual AffiliateInvoiceDTO AffiliateInvoice(int webinarID, int affiliateID)
        //{
        //    List<Order> orders = _webinarRepository.GetOrdersByWebinar(webinarID)
        //        .Where(o => o.idAffiliate == affiliateID).ToList();


        //    AffiliateInvoiceDTO reportData = BuildAffiliateInvoice(orders, webinarID, affiliateID);
        //    if (reportData != null)
        //    {
        //        return new AffiliateInvoiceDTO { idWebinar = webinarID, Affiliate = FindById(affiliateID) };
        //    }

        //    return reportData;
        //}

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
                    BuildAffiliateInvoice(new AffiliateInvoiceDTO(), orders.Where(o => o.idAffiliate == affiliate.idUserAff
                        && (o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Submitted)).OrderBy(o => o.OrderDate)
                        .ToList(), webinarID, affiliate.idUserAff);
                }


            }
            catch (Exception ex)
            {

                _logger.ErrorException("BuildAffiliateReport tossed ex: ", ex);
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



        public AffiliateInvoiceDTO BuildAffiliateInvoice(AffiliateInvoiceDTO invoice, List<Order> orders, int webinarID, int affiliateID)
        {
            return ComputeRoyalty(invoice, orders, affiliateID, true);
        }
        public AffiliateInvoiceDTO BuildAffiliateInvoiceForPostEventOrders(AffiliateInvoiceDTO invoice, List<Order> orders, int affiliateID)
        {

            return ComputeRoyaltyForPostEventOrders(invoice, orders, affiliateID, true);
        }

        private AffiliateInvoiceDTO ComputeRoyaltyForPostEventOrders(AffiliateInvoiceDTO invoice, List<Order> orders, int affiliateId, bool b)
        {

            invoice.Affiliate = FindById(affiliateId);

            int numberOfRegistrations = 0;

            switch (invoice.Affiliate.CommissionModel)
            {
                case 1: // Sliding4TierNoCCBreak:

                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        numberOfRegistrations =
                            _orderRepository.GetNumberOfOrdersPerWebinarByAffiliate(
                                order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar,
                                affiliateId);
                        decimal commissionPercent;
                        var row = IniInvoice(order, invoice);

                        if (numberOfRegistrations < 6)
                        {
                            commissionPercent = 0.3M;
                        }
                        else if (numberOfRegistrations < 11)
                        {
                            commissionPercent = 0.35M;
                        }
                        else if (numberOfRegistrations < 16)
                        {
                            commissionPercent = 0.4M;
                        }
                        else //
                        {
                            commissionPercent = 0.45M;
                        }


                        if (order.OrderStatus == OrderStatus.Paid) commissionPercent = 0.3M;

                        row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;
                    }
                    break;

                case 3: // CommissionModel.Flat40

                    numberOfRegistrations = 0;

                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        decimal commissionPercent;
                        var row = IniInvoice(order, invoice);
                        commissionPercent = 0.4M;

                        row.Royalty = row.RowPrice * commissionPercent;
                        //row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;
                    }
                    break;
                case 4: // CommissionModel.Flat35:
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {

                        var row = IniInvoice(order, invoice);
                        decimal commissionPercent = 0.35M;

                        row.Royalty = row.RowPrice * commissionPercent;
                        //row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;

                    }
                    break;
                case 5: // CommissionModel.Flat25


                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        decimal commissionPercent = 0.25M;

                        var row = IniInvoice(order, invoice);
                        row.Royalty = row.RowPrice * commissionPercent;
                        //row.Royalty = row.RowPrice * commissionPercent;
                        row.PercentPaid = commissionPercent;
                        invoice.TotalRoyalties = invoice.TotalRoyalties + row.Royalty;
                    }
                    break;

                default:
                    throw new TTSException("Invalid commission model ");
            }

            if (invoice.Affiliate.BillingModel == "aff")
            {
                invoice.TotalNetDue = (invoice.TotalOnBilled + invoice.TotalOnPaid - invoice.TotalDiscounts) - invoice.TotalRoyalties;
            }
            if (invoice.Affiliate.BillingModel == "billed")
            {
                invoice.TotalNetDue = (invoice.TotalOnBilled - invoice.TotalRoyalties) - (decimal)((double)invoice.TotalOnPaid * .03);
            }
            //When we bill:
            // (TotalOnBilled +  "TotalOnPaid" - TotalDiscount ) - TotalRoyalties = NetDueTTS

            //When Aff Bills
            //TotalOnBilled -  TotalRoyalties - ( "TotalOnPaid" * .03 )  = NetDueTTS


            //temply stored so the line item can show total being calculated
            invoice.InvoiceBody = numberOfRegistrations.ToString();
            return invoice;


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

            ComputeRoyalty(new AffiliateInvoiceDTO(), orders, idAffiliate, false);

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

                WebinarTitle = webinar.Title,
                WebinarDate = webinar.Date.ToShortDateString(),
                WebinarID = webinar.idWebinar,
                Orders = orders,

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
