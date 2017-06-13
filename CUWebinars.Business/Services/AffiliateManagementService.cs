using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System.Net;
using System.Text;
using System.Web.Razor.Generator;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using CUWebinars.Web.Models;
using CUWebinars.Web.Core;

using FluentValidation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private AffiliateInvoiceDTO ComputeRoyaltyForAdjustedOrders(AffiliateInvoiceDTO invoice, List<Order> orders, int affiliateId, bool b)
        {
            decimal totalBilledRevenue = 0M;
            decimal totalBilledRoyalty = 0M;
            decimal totalPaidRoyalty = 0M;

            invoice.Affiliate = FindById(affiliateId);

            foreach (Order order in orders.OrderBy(o => o.OrderDate))
            {
                try
                {
                    var obj = JObject.Parse(order.InvoiceDetail);
                    var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                    var dict = obj.First.First.Children().Cast<JProperty>().ToDictionary(p => p.Name, p => p.Value);

                    if (row.Discount != null)
                        _logger.Warn("GenerateWeeklyInvoicesEvent | Invoice Warning! While recalculating adjusted order a discount code was found on on idOrder: " + order.idOrder);
                    try
                    {

                        var adjustmentDirection = "Royalty is increased";

                        if (order.InvoiceDetail.Contains("decreased"))
                        {
                            adjustmentDirection = "Royalty is decreased";
                        }

                        var adjustedTotal = row.RowPrice - (decimal)dict["OriginalTotal"];
                        var adjustedRoyalty = (decimal)dict[adjustmentDirection];

                        //now increment/decriment the subtotals on 'Adjusted Orders' section.

                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            totalPaidRoyalty += adjustedRoyalty;
                        }
                        else
                        {
                            totalBilledRevenue += adjustedTotal; // 
                            totalBilledRoyalty += adjustedRoyalty;
                        }

                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            invoice.TotalOnPaid += adjustedTotal;
                        }
                        if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed || order.OrderStatus == OrderStatus.OutstandingBalance)
                        {
                            invoice.TotalOnBilled += adjustedTotal;
                        }

                        invoice.TotalRoyalties += adjustedRoyalty;

                        StoreInvoiceDetail(order, invoice);
                        _orderRepository.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("ComputRoyaltyforAdjustedOrders: " + order.idOrder, ex);
                    }

                }
                catch (Exception ex)
                {
                    _logger.FatalException("ReInvoice attempt on " + order.idOrder + " tosses: ", ex);
                }
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

        private AffiliateInvoiceDTO ComputeRoyaltyForPostEventOrders(AffiliateInvoiceDTO invoice, List<Order> orders, int affiliateId, bool b)
        {
            decimal totalBilledRevenue = 0M;
            decimal totalBilledRoyalty = 0M;
            decimal totalPaidRoyalty = 0M;

            invoice.Affiliate = FindById(affiliateId);

            int numberOfRegistrations = 0;

            switch (invoice.Affiliate.CommissionModel)
            {
                case 1: // Sliding4TierNoCCBreak:
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            var nullCheck = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                            if (nullCheck != null)
                                numberOfRegistrations =
                                    _orderRepository.GetNumberOfOrdersPerWebinarByAffiliate(
                                        nullCheck.idWebinar,
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

                            row.Royalty = row.RowPrice * commissionPercent;

                            if (row.Discount != null && row.Discount.PercentOff == 100) numberOfRegistrations--;

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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyaltyForPostEventOrders Sliding4TierNoCCBreak " + order.idOrder, ex);
                        }
                    }
                    break;

                case 2: // CommissionModel.Flat50
                    numberOfRegistrations = 0;
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            decimal commissionPercent;
                            var row = IniInvoice(order, invoice);
                            commissionPercent = 0.5M;
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


                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyaltyForPostEventOrders Flat40 " + order.idOrder, ex);
                        }

                    }
                    break;
                case 3: // CommissionModel.Flat40
                    numberOfRegistrations = 0;
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            decimal commissionPercent;
                            var row = IniInvoice(order, invoice);
                            commissionPercent = 0.4M;
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


                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyaltyForPostEventOrders Flat40 " + order.idOrder, ex);
                        }

                    }
                    break;
                case 4: // CommissionModel.Flat35:
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            var row = IniInvoice(order, invoice);
                            decimal commissionPercent = 0.35M;

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


                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyaltyForPostEventOrders Flat35 " + order.idOrder, ex);
                        }
                    }
                    break;
                case 5: // CommissionModel.Flat25
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            decimal commissionPercent = 0.25M;

                            var row = IniInvoice(order, invoice);
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


                            StoreInvoiceDetail(order, invoice);

                        }
                        catch (Exception ex)
                        {

                            _logger.FatalException("ComputRoyaltyForPostEventOrders Flat25 " + order.idOrder, ex);
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
                        try
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

                            if (row.Discount != null && row.Discount.PercentOff == 100) numberOfRegistrations--;

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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyalty Sliding4TierNoCCBreak " + order.idOrder, ex);
                        }
                    }
                    break;

                case 2: // CommissionModel.Flat50
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            decimal commissionPercent;
                            var row = IniInvoice(order, invoice);
                            commissionPercent = 0.5M;

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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyalty Flat40 " + order.idOrder, ex);
                        }
                    }
                    break;

                case 3: // CommissionModel.Flat40
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            decimal commissionPercent;
                            var row = IniInvoice(order, invoice);
                            commissionPercent = 0.4M;

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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyalty Flat40 " + order.idOrder, ex);
                        }
                    }
                    break;

                case 4: // CommissionModel.Flat35:
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
                        {
                            var row = IniInvoice(order, invoice);
                            decimal commissionPercent = 0.35M;

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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyalty Flat35 " + order.idOrder, ex);
                        }

                    }
                    break;

                case 5: // CommissionModel.Flat25
                    foreach (Order order in orders.OrderBy(o => o.OrderDate))
                    {
                        try
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

                            StoreInvoiceDetail(order, invoice);
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("ComputRoyalty Sliding4TierNoCCBreak " + order.idOrder, ex);
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

        private void StoreInvoiceDetail(Order order, AffiliateInvoiceDTO invoice)
        {
            try
            {
                var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                if (!ReferenceEquals(row, null))
                {
                    var newJson = new JProperty("OrderIsInvoiced",

                        new JObject(
                            new JProperty("InvoiceId", invoice.InvoiceID),
                            new JProperty("DateOfInvoice",
                                TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                            new JProperty("AmountOfOrder", order.Total),
                            new JProperty("AmountOfRoyalty", row.Royalty),
                            new JProperty("PercentPaid", row.PercentPaid),
                            new JProperty("Affiliate", _affiliateRepository.FindById(order.idAffiliate).ttsDomain)
                            ));

                    order.InvoiceDetail = JsonHelpers.ReplaceJsonWithStoredField(order.InvoiceDetail, newJson, "OrderIsInvoiced");
                    _logger.Info("GenerateWeeklyInvoicesEvent | StoreInvoiceDetail on idOrder: " + order.idOrder);
                    _logger.Info("GenerateWeeklyInvoicesEvent : UPDATE dbo.[Order] SET InvoiceDetail = '" + order.InvoiceDetail + "' where idOrder =" + order.idOrder);

                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("GenerateWeeklyInvoicesEvent | StoreInvoiceDetail on idOrder: " + order.idOrder, ex);
            }
            _orderRepository.SaveChanges();
        }


        private OrderRow IniInvoice(Order order, AffiliateInvoiceDTO invoice)
        {

            OrderRow row = order.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            if (row == null) throw new ArgumentNullException("row");
            if (order.OrderStatus == OrderStatus.Paid)
            {
                invoice.TotalOnPaid += row.RowPrice;
            }
            if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed || order.OrderStatus == OrderStatus.OutstandingBalance)
            {
                invoice.TotalOnBilled += row.RowPrice;
                //_logger.Info("TotalOnBilled =  " + invoice.TotalOnBilled);
                order.OrderStatus = OrderStatus.Billed;
            }


            if (row.Discount != null)
            {
                if (row.Discount.PercentOff > 0)
                {
                    invoice.TotalDiscounts = invoice.TotalDiscounts + (row.UnitPrice * ((row.Discount.PercentOff) / 100));
                    _logger.Info("GenerateWeeklyInvoicesEvent | IniOrder | Discount" + invoice.Affiliate.idUserAff + "-" + row.idOrder + "-" + (row.UnitPrice * ((row.Discount.PercentOff) / 100)));

                }
                if (row.Discount.FlatOff > 0)
                {
                    invoice.TotalDiscounts = invoice.TotalDiscounts + row.UnitPrice - row.Discount.FlatOff;
                    _logger.Info("GenerateWeeklyInvoicesEvent | IniOrder | Discount" + invoice.Affiliate.idUserAff + "-" + row.idOrder + "-" + (row.UnitPrice * ((row.Discount.PercentOff) / 100)));
                }
            }
            return row;
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
                    BuildAffiliateInvoice(new AffiliateInvoiceDTO(), orders.Where(o => o.idAffiliate == affiliate.idUserAff
                        && (o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Submitted || o.OrderStatus == OrderStatus.OutstandingBalance)).OrderBy(o => o.OrderDate)
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

        public AffiliateInvoiceDTO BuildAffiliateInvoiceForAdjustedOrders(AffiliateInvoiceDTO invoice, List<Order> theseOrders,
            int thisAffiliate)
        {
            return ComputeRoyaltyForAdjustedOrders(invoice, theseOrders, thisAffiliate, true);

        }


        //public AffiliateInvoiceDTO GetAffiliateInvoice(int idWebinar, string aff)
        //{

        //    decimal _totalDue = 0;
        //    decimal _totalOnBilled = 0;
        //    decimal _totalDiscounts = 0;
        //    decimal _totalOnPaid = 0;
        //    decimal _totalRoyalties = 0;
        //    int idAffiliate = Convert.ToInt32(aff);
        //    Webinar webinar = _webinarRepository.FindById(idWebinar);
        //    var orders = _orderRepository.GetOrdersByWebinar(idWebinar).Where(a => a.idAffiliate == idAffiliate).ToList();
        //    IList<OrderRowModel> rows = new List<OrderRowModel>();

        //    ComputeRoyalty(new AffiliateInvoiceDTO(), orders, idAffiliate, false);

        //    var ordinalHolder = 0;
        //    foreach (var order in orders)
        //    {
        //        ordinalHolder++;
        //        var orow = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
        //        OrderRowModel row = new OrderRowModel
        //        {
        //            Royalty = orow.Royalty.ToString(),
        //            Email = order.BillingEmail,
        //            Institution = order.Institution,
        //            Name = order.FirstName + " " + order.LastName,
        //            OrderID = order.idOrder.ToString(),
        //            Percent = GetRowPercent(ordinalHolder, FindById(idAffiliate).CommissionModel),
        //            Price = order.Total.ToString(),
        //            Discount = (orow.UnitPrice - orow.RowPrice).ToString(),
        //            Status = order.OrderStatus.ToString()
        //        };

        //        if (order.OrderStatus == OrderStatus.Paid)
        //        {
        //            _totalOnPaid += order.Total;
        //        }
        //        if (order.OrderStatus == OrderStatus.Submitted)
        //        {
        //            _totalOnBilled += order.Total;
        //        }
        //        _totalDue += order.Total;
        //        _totalRoyalties += orow.Royalty;

        //        rows.Add(row);


        //    }

        //    var model = new AffiliateInvoiceDTO
        //    {
        //        Affiliate = _affiliateRepository.FindById(idAffiliate),
        //        AffiliateName = _affiliateRepository.FindById(idAffiliate).DisplayTitle,

        //        WebinarTitle = webinar.Title,
        //        WebinarDate = webinar.Date.ToShortDateString(),
        //        WebinarID = webinar.idWebinar,
        //        Orders = orders,

        //        TotalDiscounts = _totalDiscounts,
        //        TotalNetDue = _totalDue,
        //        TotalOnBilled = _totalOnBilled,
        //        TotalOnPaid = _totalOnPaid,
        //        TotalRoyalties = _totalRoyalties

        //    };
        //    return model;
        //}

        public IList<DiscountDTO> GetSubscriptionsByAffiliate(int idUserAff)
        {
            IList<DiscountDTO> theseSubscriptions = new List<DiscountDTO>();

            if (idUserAff != 19)
            {
                var theseDiscounts = _context.Discounts
                    .Where(d => (d.DiscountType == DiscountType.Subscription || d.DiscountType == DiscountType.ComplianceSeries) && d.idAffiliate == idUserAff).ToList();

                foreach (var discount in theseDiscounts)
                {
                    IList<WebUser> wpsUsers = _context.WebUsers.Where(u => u.idSubscriptionDiscount == discount.idDiscount).ToList();
                    var userEmail = "";
                    var order = _context.Orders.SingleOrDefault(o => o.idOrder == discount.idDiscount);
                    if (order != null)
                    {

                        userEmail = order.BillingEmail;
                    }

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
                        Status = discount.Status,
                        DiscountType = discount.DiscountType,
                        CreditsRemain = _orderRepository.CalculateCreditsRemain(discount),
                        CreditsUsed = _orderRepository.CalculateCreditsUsed(discount),
                        UserEmail = userEmail,
                        WpsUsers = wpsUsers
                    };
                    theseSubscriptions.Add(thisSub);
                }

            }
            else
            {
                var theseDiscounts = _context.Discounts
                    .Where(d => (d.DiscountType == DiscountType.Subscription || d.DiscountType == DiscountType.ComplianceSeries)
                    ).ToList();

                foreach (var discount in theseDiscounts)
                {
                    IList<WebUser> wpsUsers = _context.WebUsers.Where(u => u.idSubscriptionDiscount == discount.idDiscount)
                        .ToList();
                    var userEmail = "";
                    var order = _context.Orders.SingleOrDefault(o => o.idOrder == discount.idDiscount);
                    if (order != null)
                    {

                        userEmail = order.BillingEmail;
                    }

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
                        Status = discount.Status,
                        DiscountType = discount.DiscountType,
                        CreditsRemain = _orderRepository.CalculateCreditsRemain(discount),
                        CreditsUsed = _orderRepository.CalculateCreditsUsed(discount),
                        UserEmail = userEmail,
                        WpsUsers = wpsUsers
                    };
                    theseSubscriptions.Add(thisSub);
                }
            }

            return theseSubscriptions;
        }

        public List<Uri> GetInvoicesByAffiliate(string tenant, int idAffiliate)
        {

            List<Uri> links = new List<Uri>();
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            System.Globalization.Calendar cal = dfi.Calendar;
            var urlBase = "https://storeforcu.blob.core.windows.net/affiliateinvoices/";

            if (tenant == "BankWebinars") urlBase = "https://storeforbw.blob.core.windows.net/affiliateinvoices/";

            var firstMonday = Core.Extensions.DateTimeExtensions.ToDateTime("1/2/2017");

            if (DateTime.Now.Year == 2017)
            {
                for (var _week = 55; _week >= 0; _week--)
                {
                    //https://storeforbw.blob.core.windows.net/affiliateinvoices/10-17-2016/43-2016-11464.pdf
                    DateTime theMonday;
                    if (_week == 0)
                    {
                        theMonday = firstMonday.Value;
                    }
                    else
                    {
                        theMonday = firstMonday.Value.AddDays(_week * 7);
                    }
                    var weekNumber = cal.GetWeekOfYear(theMonday, dfi.CalendarWeekRule,
                                         dfi.FirstDayOfWeek) + "-" + cal.GetYear(DateTime.Now);

                    //Console.WriteLine("The current date and time: {0:MM/dd/yy H:mm:ss zzz}",thisDate2);
                    string theFileName = theMonday.ToString("M/d/yyyy").Replace("/", "-") + "/" + weekNumber + "-" +
                                         idAffiliate + ".pdf";
                    string theURL = urlBase + theMonday.ToString("M/d/yyyy").Replace("/", "-") + "/" + weekNumber + "-" + idAffiliate + ".pdf";

                    Uri blob = null;

                    try
                    {
                        var blobTest = BlobHelper.GetBlob("invoicesprivate/", theMonday.ToString("M/d/yyyy").Replace("/", "-"), weekNumber + "-" + idAffiliate + ".pdf");
                        if (blobTest != null)
                        {
                            blob = Core.Helpers.BlobHelper.GetInvoiceForPage(theFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("CheckForExistingInvoice: ", ex);
                    }
                    if (blob != null)
                        links.Add(blob);
                }
            }

            firstMonday = Core.Extensions.DateTimeExtensions.ToDateTime("1/4/2016");
            for (var _week = 55; _week >= 0; _week--)
            {
                //https://storeforbw.blob.core.windows.net/affiliateinvoices/10-17-2016/43-2016-11464.pdf
                DateTime theMonday;
                if (_week == 0)
                {
                    theMonday = firstMonday.Value;
                }
                else
                {
                    theMonday = firstMonday.Value.AddDays(_week * 7);
                }
                var weekNumber = cal.GetWeekOfYear(theMonday, dfi.CalendarWeekRule,
                                     dfi.FirstDayOfWeek) + "-2016";

                //Console.WriteLine("The current date and time: {0:MM/dd/yy H:mm:ss zzz}",thisDate2);
                string theFileName = theMonday.ToString("M/d/yyyy").Replace("/", "-") + "/" + weekNumber + "-" +
                                     idAffiliate + ".pdf";
                string theURL = urlBase + theMonday.ToString("M/d/yyyy").Replace("/", "-") + "/" + weekNumber + "-" + idAffiliate + ".pdf";

                Uri blob = null;
                try
                {
                    var blobTest = BlobHelper.GetBlob("invoicesprivate/", theMonday.ToString("M/d/yyyy").Replace("/", "-"), weekNumber + "-" + idAffiliate + ".pdf");
                    if (blobTest != null)
                    {
                        blob = Core.Helpers.BlobHelper.GetInvoiceForPage(theFileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.FatalException("CheckForExistingInvoice: ", ex);
                }
                if (blob != null)
                    links.Add(blob);
            }


            return links;

        }


        public IList<Uri> GetPromosByAffiliate(string tenant, int idAffiliate, int idWebinar)
        {
            List<Uri> links = new List<Uri>();

            //https://storeforbw.blob.core.windows.net/affiliateinvoices/10-17-2016/43-2016-11464.pdf

            var urlBase = "https://storeforcu.blob.core.windows.net/v3generator/";

            if (tenant == "BankWebinars") urlBase = "https://storeforbw.blob.core.windows.net/v3generator/";
            for (int x = 0; x < 6; x++)
            {
                string theFileName = idWebinar.ToString();
                string theExt = ".html";
                switch (x)
                {
                    case 0:
                        theExt = ".pdf";
                        break;
                    case 1:
                        theExt = ".docx";
                        break;
                    case 2:
                        theExt = ".txt";
                        break;
                    case 3:
                        theExt = ".html";
                        break;
                    case 4:
                        theFileName = "_promo" + idWebinar;
                        theExt = ".html";
                        break;

                }
                //Console.WriteLine("The current date and time: {0:MM/dd/yy H:mm:ss zzz}",thisDate2);

                Uri blob = null;
                try
                {
                    var blobTest = BlobHelper.GetBlob("v3generator/", idAffiliate.ToString(), theFileName + theExt);

                    if (blobTest != null)
                    {
                        blob = Core.Helpers.BlobHelper.GetPromosForPage(idAffiliate + "/" + theFileName + theExt);
                    }
                }
                catch (Exception ex)
                {
                    _logger.FatalException("CheckForExistingInvoice: ", ex);
                }
                if (blob != null)
                    links.Add(blob);
            }
            return links;

        }

        public int SaveChanges(Affiliate affiliate)
        {// 12/15 not currently working
            return _affiliateRepository.SaveChanges();
        }

        public Affiliate UpdateAffiliate(Affiliate _affiliate)
        {

            //get db version of aff
            var affiliate = FindById(_affiliate.idUserAff);

            var sb = new StringBuilder();
            sb.Append("AffiliateUpDate");

            if (affiliate.WebUser.timeZone != _affiliate.WebUser.timeZone)
                sb.Append(" TimeZone changed from: " + affiliate.WebUser.timeZone + " to: " +
                          _affiliate.WebUser.timeZone);

            if (affiliate.idMailChimpList != _affiliate.idMailChimpList)
                sb.Append(" idMailChimpList changed from: " + affiliate.idMailChimpList + " to: " +
                          _affiliate.idMailChimpList);

            if (affiliate.BillingModel != _affiliate.BillingModel)
                sb.Append(" BillingModel changed from: " + affiliate.BillingModel + " to: " +
                          _affiliate.BillingModel);
            if (affiliate.PromoSenderEmail != _affiliate.PromoSenderEmail)
                sb.Append(" PromoSenderEmail changed from: " + affiliate.PromoSenderEmail + " to: " +
                          _affiliate.PromoSenderEmail);

            if (affiliate.PromoSenderName != _affiliate.PromoSenderName)
                sb.Append(" PromoSenderName changed from: " + affiliate.BillingModel + " to: " +
                          _affiliate.BillingModel);

            DataOperations ops = new DataOperations(TtsConfig.DefaultConnectionString);
            var saveAff = ops.UpdateAffiliate(_affiliate);

            return affiliate;

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
                        if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.OutstandingBalance || order.OrderStatus == OrderStatus.Billed)
                        {
                            registration.TotalOnBilled += row.RowPrice;

                        }
                        if (order.OrderStatus == OrderStatus.Paid)
                        {
                            registration.TotalOnPaid += row.RowPrice;
                        }
                    }

                    if (order.OrderStatus == OrderStatus.Submitted || order.OrderStatus == OrderStatus.Billed || order.OrderStatus == OrderStatus.OutstandingBalance ||
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


