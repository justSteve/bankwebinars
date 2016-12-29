using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Cache;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Core.Extensions;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using CUWebinars.Web.Core.Cache;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BrockAllen.MembershipReboot;
using Citrix.GoToWebinar.Api;
using Citrix.GoToWebinar.Api.Model;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Notification;
using CUWebinars.Web.Models;
using Ninject.Infrastructure.Language;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;
using IEventSource = CUWebinars.NotificationSystem.Event.IEventSource;
using Webinar = CUWebinars.Business.Models.Webinar;

namespace CUWebinars.Business.Services
{
    public class OrderManagementService : IEventSource, IOrderManagementService
    {
        private readonly ICachingService _cachingService;
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IRegTypeRepository _regTypeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IAdditionalLocationsRepository _additionalLocationsRepository;
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly TtsConfiguration _ttsConfig;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public OrderManagementService(
            IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
            IDiscountRepository discountRepository,
            IWebinarRepository webinarRepository,
            IAdditionalLocationsRepository additionalLocationsRepository,
            ILogger logger,
            TtsConfiguration ttsConfig)
        {
            _affiliateRepository = affiliateRepository;
            _regTypeRepository = regTypeRepository;
            _orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
            _webinarRepository = webinarRepository;
            _discountRepository = discountRepository;
            _additionalLocationsRepository = additionalLocationsRepository;
            _logger = logger;
            _webUserRepository = webUserRepository;
            _cachingService = new OrderCachingService();
        }

        public void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation)
        {
            _orderRepository.AddAdditionalLocation(addedAdditionalLocation);
        }
        //public string ApplyDiscountCode(int? discountId)
        //{
        //    _logger.Fatal("not implemented: " + discountId.Value);
        //    return null;
        //}

        public Order AssignAffiliateToOrder(int affiliateId, Order order)
        {
            return _orderRepository.AssignAffiliate(affiliateId, order);
        }

        public Order AssignWebUserToOrder(WebUser webUser, Order order)
        {
            if (webUser == null) throw new ArgumentNullException("webUser");
            if (order == null) throw new ArgumentNullException("order");

            return _orderRepository.AssignWebUserToOrder(webUser, order);
        }

        public Affiliate AttachAffiliate(Affiliate item)
        {
            item.Orders = null;
            return _affiliateRepository.Exists(item) ? item : _affiliateRepository.AttachItem(item);
        }

        //public Order CreateNewOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null)
        //{
        //    var order = _orderRepository.CreateOrder(GetAffiliateById(affiliateId), webUser, webinar, orderRow, origin);
        //    var email = webUser == null ? "notauthenticated@cuwebinars.com" : webUser.email;

        //    //_logger.Info("CreateNewOrder: " + email + " | " + orderRow.Webinar.Title + " | " + orderRow.RegistrationType.OptionLabel);
        //    return order;

        //}


        public Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null)
        {
            var order = _orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow, origin);
            var email = webUser == null ? "notauthenticated@cuwebinars.com" : webUser.email;

            _logger.Info("CreateNewOrder: " + email + " | " + orderRow.Webinar.Title + " | " + orderRow.RegistrationType.OptionLabel);
            return order;
        }

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType)
        {
            try
            {
                var regType = _regTypeRepository.FindRegType(registrationType);

                if (additionalLocation != null)
                {
                    foreach (var addLoc in additionalLocation)
                    {
                        addLoc.Price = GetCostOfAdditionalLocations(additionalLocation, webinar.idWebinar).Item2;
                    }
                }

                OrderRow row = _orderRepository.CreateOrderRow(webinar, additionalLocation, regType);

                return row;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("overload of CreateOrderRow method webinar:  " + webinar.idWebinar + " regType: " + registrationType, exception);

                throw;
            }
        }

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType)
        {
            try
            {
                _orderRepository.CreateOrderRow(webinar, additionalLocation, registrationType);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("CreateOrderRow method webinar:  " + webinar.idWebinar + " regType: " + registrationType.idRegType, exception);

            }

            return null;
        }


        public AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email paramter was either null or white space.", "email");

            try
            {
                return _orderRepository.CreateAdditionalLocation(email, price, fullname);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("CreateAdditionalLocation method - values passed in {0},{1},{2}.", email, price, fullname), exception);
                throw;
            }
        }


        public IEnumerable<AdditionalLocation> GetAdditionalLocationsForOrderRow(int idOrderRow)
        {
            return _additionalLocationsRepository.GetAdditionalLocationsForOrderRow(idOrderRow);
        }

        public IDictionary<int, string> GetAffiliatesForDisplayList()
        {
            return _affiliateRepository.GetAll()
                .ToDictionary(a => a.idUserAff, a => a.ttsDomain);
        }

        public Tuple<string, decimal> GetCostOfAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations, int idWebinar)
        {
            //so renamed to reflect that we are building the cost of a user's list of added seats. Not
            // how much does it cost per seat.
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var addresses = new StringBuilder();
            var i = 0;

            var additionalLocationsPricing = GetAdditionalLocationsPricing(idWebinar);

            decimal optionsCost = 0M;

            // perf tweak - ensures enumerable will only be enumerated once
            var additionalLocationsEnumerated = additionalLocations as AdditionalLocation[] ?? additionalLocations.ToArray();
            //var additionalLocationsEnumerated = additionalLocations as AdditionalLocation[] ?? additionalLocations.ToArray();

            string emailSpanElement = string.Empty; // emails in bold text, or whatever suits
            //tagBuilder.AddCssClass("muted");

            foreach (var additionalLocation in additionalLocationsEnumerated)
            {
                emailSpanElement = string.Format("<strong>{0}</strong>", additionalLocation.Email);

                i++;

                if (i == additionalLocationsEnumerated.Count())
                {
                    addresses.Append(emailSpanElement);
                }
                if (i < additionalLocationsEnumerated.Count())
                {
                    if (i == additionalLocationsEnumerated.Count() - 1)
                    {
                        addresses.Append(emailSpanElement + " and ");
                    }
                    else
                    {
                        addresses.Append(emailSpanElement + ", ");
                    }
                }
            }

            optionsCost = additionalLocationsPricing;

            return new Tuple<string, decimal>(addresses.ToString(), optionsCost);
        }

        public Affiliate GetAffiliateByDomain(string domain)
        {
            return _affiliateRepository.LoadByTTSDomain(domain);
        }

        public Affiliate GetAffiliateById(int id)
        {
            return _affiliateRepository.FindById(id);
        }

        public Affiliate GetAffiliateByIdLoaded(int id, params Expression<Func<Affiliate, object>>[] includeProperties)
        {
            //var aff = _affiliateRepository.FindByIdWithIncluding(id, includeProperties);
            var aff = _affiliateRepository.FindByIdWithIncluding(id);
            if (aff == null)
            {

                _logger.Warn("Error: unfound affiliate=" + id);
                aff = GetAffiliateById(19);
            }
            return aff;
        }


        public IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached)
        {
            string cachKey = "options-" + id;
            //var options = _cachingService.Get(cachKey);

            //if (options == null)
            //{
            var options = _regTypeRepository.FindRegTypesByWebinarId(id, false);

            //    // keeps options object in cache for 1 hour.
            //    _cachingService.Add(cachKey, options, DomainConstants.BuildUtcNowAsCts.AddHours(1));
            //}

            return (IDictionary<RegType, bool>)options;
        }

        public IDictionary<RegType, bool> GetAllPossibleOptionsByWebinarId(int idWebinar, bool detached)
        {
            string cachKey = "options-" + idWebinar;
            var options = _cachingService.Get(cachKey);

            //if (options == null)
            //{
            options = _regTypeRepository.FindAllPossibleRegTypesByWebinarId(idWebinar, false);

            //    // keeps options object in cache for 1 hour.
            //    _cachingService.Add(cachKey, options, DomainConstants.BuildUtcNowAsCts.AddHours(1));
            //}

            return (IDictionary<RegType, bool>)options;
        }


        public IEnumerable<Order> GetOrdersByEmail(string email, int aff)
        {
            return _orderRepository.FindOrdersByBillingEmail(email.Trim(), aff);
        }

        public Order FindExpressCheckoutOrder(string email, int idWebinar)
        {
            var order = _orderRepository.FindExpressCheckoutOrder(email.Trim(), idWebinar);
            var regType = GetRegTypeOfOrderRow(order.OrderRows.FirstOrDefault().idRegType);
            var webinar = GetWebinarById(order.OrderRows.FirstOrDefault().idWebinar);
            order.OrderRows.FirstOrDefault(o => o.RowStatus == OrderRowStatus.Active).RegistrationType = regType;
            order.OrderRows.FirstOrDefault(o => o.RowStatus == OrderRowStatus.Active).Webinar = webinar;
            return order;
        }

        //public IEnumerable<Order> GetOrdersByEmailDomain(string email, int aff)
        //{
        //    return _orderRepository.FindOrdersByBillingEmailDomain(email.Trim(), aff);
        //}

        public void SendAdhocNotification(string emails, string subject, string body)
        {
            var adhocNotificationSubmittedViewModel = new AdhocNotificationMessage
            {
                Body = body,
                Recipients = emails.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                Subject = subject,
            };

            FireAdhocNotificationHandler(adhocNotificationSubmittedViewModel);
        }

        public Order FindExpressCheckoutOrderByOrderId(int q11Orderid)
        {
            return _orderRepository.GetOrderById(q11Orderid);
        }

        public IEnumerable<Order> GetV3OrdersByWebinarForPostEventClaims(int idWebinar)
        {

            return _webinarRepository.GetOrdersByWebinarForPostEventClaims(idWebinar);
        }


        public IEnumerable<Order> GetOrdersByLastName(string lastName, int aff)
        {
            return _orderRepository.FindOrdersByLastName(lastName, aff);
        }


        public IList<Order> GetOrdersForLiveNotifications(int idWebinar)
        {
            return _orderRepository.GetOrdersForLiveEventNotifications(idWebinar);
        }

        public IList<Order> GetOrdersForRecordedNotifications(int idWebinar)
        {
            return _orderRepository.GetOrdersForRecordedEventNotifications(idWebinar);
        }

        public IList<RegType> GetShowRecordingNotificationsForWebinar(int idWebinar)
        {
            return null;
        }

        public IDictionary<RegType, bool> GetRegTypesByWebinarId(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
        }

        public RegType GetRegTypeOfOrderRow(int idRegType)
        {
            return _regTypeRepository.FindRegType(idRegType);
        }

        public IList<RegType> GetRegTypeOption(int optionId)
        {
            return _regTypeRepository.FindRegTypeOption(optionId);
        }

        public WebUser GetWebUser(string email)
        {
            if (email == null) throw new ArgumentNullException("email");

            return _webUserRepository.GetWebUserByEmail(email);
        }

        public OrderRow GetOrderRowById(int idOrderRow)
        {
            return _orderRepository.GetOrderRowById(idOrderRow);
        }

        public IEnumerable<Order> GetOrdersForShippedNotification()
        {
            return _orderRepository.GetOrdersForShippedEventNotifications();
        }

        public Order GetOrderById(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            return order;
        }

        public Order GetOrderByIdThin(int id)
        {
            return _orderRepository.GetOrderByIdThin(id);
        }

        public IEnumerable<int> GetOrderIdsByPartialId(int id)
        {
            return _orderRepository.FindOrderIdsByPartialId(id);
        }
        public IEnumerable<int> GetUserIdsByPartialId(int value)
        {
            return _orderRepository.FindUserIdsByPartialId(value);
        }

        public object SearchRegistrations(int affiliateID, IList<int> excludeUserIDs, int skip, int take, string search)
        {
            return
                _orderRepository.SearchOrders(affiliateID, excludeUserIDs, skip, take, search);
        }

        public bool VerifyWebUserExists(int idUser)
        {
            return _webUserRepository.WebUserExists(idUser);
        }

        public Webinar GetWebinarByJoinCode(string joinCode)
        {
            return _webinarRepository.GetWebinarByJoinCode(joinCode);
        }

        public void LoadWebinarIntoOrderRow(OrderRow newOrderRow)
        {
            _orderRepository.LoadWebinarIntoOrderRow(newOrderRow);
        }

        public void SetUserStatusToUnChanged(WebUser user)
        {
            _webUserRepository.SetUserStatusToUnChanged(user);
        }

        public void SetAffiliateStatusToUnChanged(Affiliate affiliate)
        {
            _affiliateRepository.SetAffiliateStatusToUnChanged(affiliate);
        }




        public IList<Order> GetOrdersByUserId(int id)
        {
            try
            {
                return _orderRepository.FindOrdersByUserIdWithOrderRows(id);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetOrdersByUserId = " + id, exception);
                return null;
            }
        }

        public IEnumerable<Order> GetOrdersAll(int idAffliate, out int totalNumberOrders)
        {
            return _orderRepository.GetOrdersAll(idAffliate, out totalNumberOrders);
        }

        public IEnumerable<Order> GetOrdersAllForInvoice(int idAffliate, out int totalNumberOrders)
        {
            return _orderRepository.GetOrdersAllForInvoice(idAffliate, out totalNumberOrders);
        }

        public IEnumerable<Discount> GetSubscriptionsAll(int idAffliate, out int totalNumberOrders)
        {
            return _orderRepository.GetSubscriptionsAll(idAffliate, out totalNumberOrders);
        }

        public WebUser GetWebUser(int id)
        {
            return _webUserRepository.FindByIdLoaded(id);
        }

        public IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar)
        {
            return _webUserRepository.GetWebusersForLiveNotifications(idWebinar);
        }

        public string GetWebUserFullname(string email)
        {
            return _webUserRepository.GetWebUserFullname(email);
        }

        public void DeleteOrder(int orderId)
        {
            var order = GetOrderById(orderId);

            _orderRepository.DeleteOrder(order);
        }

        public Affiliate DetermineAffiliateByAlternativeMeans(int idUser)
        {
            string cachKey = "webUserId-" + idUser;
            //var affiliateIds = _cachingService.Get(cachKey) as IList<int>;
            IList<int> affiliateIds = null;

            if (affiliateIds == null)
            {
                //affiliateIds = _orderRepository.FindOrdersByUserId(idUser)
                //                    .OrderByDescending(o => o.OrderDate)
                //                    .Select(o => o.idAffiliate)
                //                    .ToList();
                ////
                //HACK: eliminate the problematic calculations and hard-wire the
                // resultset to include only 1 affiliate chosen by most recent order.
                affiliateIds = _orderRepository.FindOrdersByUserId(idUser)
                                    .OrderByDescending(o => o.OrderDate)
                                    .Select(o => o.idAffiliate)

                                    .ToList();

                // keeps affiliateIds object in cache for 1 hour.
                //_cachingService.Add(cachKey, affiliateIds, DomainConstants.BuildUtcNowAsCts.AddHours(1));
            }

            if (affiliateIds.Any())
            {
                _logger.Info("DetermineAffiliateByAlternativeMeans found at one aff: " + idUser);

                //  get the most recent
                int affiliateIdForOrder, mostRecentAffiliateId;
                affiliateIdForOrder = mostRecentAffiliateId = affiliateIds.First();

                var sb = new StringBuilder();
                // The history is of more than 1 affiliate
                if (affiliateIds.Distinct().Count() > 1)
                {
                    _logger.Info("DetermineAffiliateByAlternativeMeans finds multi affs: " + idUser);

                    // The business rule is that where there is more than one Affiliate which the 
                    // user has made orders for, if one affiliate has been used twice as many times 
                    // as the most recent Affiliate, then make the order for that Affiliate.
                    var groups = affiliateIds.GroupBy(a => a);
                    //
                    int mostUsedAffiliateId = 0;
                    int mostUses = 0;
                    int numberOfUsesOfMostRecentAffiliate = 0;
                    int current = 0;
                    foreach (var group in groups)
                    {
                        sb.Append(group.Key + ", ");
                        current = group.Count();

                        if (current > mostUses)
                        {
                            mostUses = current;
                            mostUsedAffiliateId = group.Key;
                        }

                        if (group.Key == mostRecentAffiliateId)
                        {
                            numberOfUsesOfMostRecentAffiliate = current;
                        }
                    }

                    if (mostUses >= 2 * numberOfUsesOfMostRecentAffiliate)
                    {
                        _logger.Info("DetermineAffiliateByAlternativeMeans by mostUses: " + idUser);
                        affiliateIdForOrder = mostUsedAffiliateId;
                    }
                    _logger.Error("multiple affiliates considered: {0} for idUser {1}. Credited to {2}.", sb.ToString(), idUser, affiliateIdForOrder, current);

                }

                //cachKey = "affiliateId-" + affiliateIdForOrder;
                var affiliate = _cachingService.Get(cachKey) as Affiliate;

                if (affiliate == null)
                {
                    affiliate = _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder); // use the most recent
                    //affiliate = _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder, a => a.WebUser); // use the most recent
                    if (affiliate == null)
                    {
                        affiliate = GetAffiliateById(19);
                    }
                    // keeps Affiliate object in cache for 1 hour.
                    //_cachingService.Add(cachKey, affiliate, DomainConstants.BuildUtcNowAsCts.AddHours(1));
                }
                _logger.Info("DetermineAffiliateByAlternativeMeans returned: " + affiliate.idUserAff + " for: " + idUser);

                return affiliate;
            }
            return null;
        }

        public void DispatchDummyOrder()
        {
            foreach (var orderSubmittedEvent in GetEvents())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(orderSubmittedEvent);
            }
        }

        public IEnumerable<Order> FindOrdersByUserId(int userId)
        {
            return _orderRepository.GetOrdersByUserId(userId);
        }

        public void FireAdhocNotificationHandler(AdhocNotificationMessage adhocNotificationMessage)
        {
            AddEvent(new AdhocNotificationEvent<AdhocNotificationMessage>
            {
                EventObject = adhocNotificationMessage
            });

            foreach (var evt in GetEvents().OfType<AdhocNotificationEvent<AdhocNotificationMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            //int rowsUpdated = _orderRepository.SaveChanges();
        }

        public OrderRow LoadOrderRow(int id)
        {
            try
            {
                using (var context = new TTSWebinarsContext())
                {
                    return context.OrderRows.Single(o => o.idOrderRow == id);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.FatalException("ERROR at LoadOrderRow=" + id, ex);
                throw new EntityNotFoundException(ex.Message, ex);
            }
        }

        public int SaveChanges()
        {
            return _orderRepository.SaveChanges();
        }

        public Order LoadOrder(int id)
        {
            try
            {
                using (var context = new TTSWebinarsContext())
                {
                    return context.Orders.Single(o => o.idOrder == id);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.FatalException("ERROR at LoadOrder=" + id, ex);
                throw new EntityNotFoundException(ex.Message, ex);
            }
        }
        //public static WebUser GetMasterUser(this UserFacade userFacadeInstance)
        //{
        //    if (userFacadeInstance.IsCurrentUserControlled() == false)
        //    {
        //        throw new TTSException("There is no controlled user");
        //    }

        //    return GetAuthenticatedUser(userFacadeInstance);
        //}

        public IEnumerable<IEvent> GetEvents()
        {
            return _events;
        }

        protected void AddEvent<TE>(TE orderEvent) where TE : IEvent
        {
            if (orderEvent is IAllowMultiple || _events.All(x => x.GetType() != orderEvent.GetType()))
            {
                _events.Add(orderEvent);
            }
        }
        public PricesAndDiscounts CalculateOrderCost(Order order, decimal optionsCost)
        {
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            decimal totalOptionsPrice = 0M;

            var row = order.OrderRows.SingleOrDefault(orderRow => orderRow.RowStatus == OrderRowStatus.Active);

            Debug.Assert(row != null, "OrderRow object should always have a value here.");
            if (row.RegistrationType != null)
            {
                row.UnitPrice = (decimal)row.RegistrationType.Price;
            }
            else
            {
                _logger.Warn("COC did not find row when processing " + order.idOrder);
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                var regTypePricing = dataOperations.GetCostOfRegtype(row.idRegType);
                row.UnitPrice = regTypePricing;

            }
            //Calculate row price before discount
            if (row.AdditionalLocation != null)
            {
                if (order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active).Webinar.Title.Contains("Compliance Perspectives"))
                {
                    if (row.AdditionalLocation.Count > 3)
                    {
                        totalOptionsPrice = row.AdditionalLocation.Count - 3 * optionsCost; // cost * number of additional locations
                    }
                }
                else
                {
                    totalOptionsPrice = row.AdditionalLocation.Count * optionsCost; // cost * number of additional locations
                }
            }

            row.RowPrice = row.UnitPrice + totalOptionsPrice;
            pricesAndDiscounts.UnitPrice = row.UnitPrice;

            //Calculate discount. 
            decimal discountTotal = 0;

            if (row.Discount != null && row.Discount.PercentOff != 0.0M)
            {
                var creditsRemain = CalculateCreditsRemain(row.Discount);

                if (row.Discount.DiscountType == DiscountType.Subscription)
                {
                    _logger.Info("COC begin discount create " + row.Discount.idDiscount + " on " + row.idOrder + " found "
                            + creditsRemain + " against " + row.RegistrationType.CreditCost);

                    if (creditsRemain >= row.RegistrationType.CreditCost)
                    {
                        discountTotal = row.RowPrice * row.Discount.PercentOff / 100;
                        _logger.Info("COC create discount  " + row.Discount.idDiscount + " on " + row.idOrder + " found "
                            + creditsRemain + " against " + row.RegistrationType.CreditCost);
                    }
                    else
                    {
                        if (creditsRemain >= 0)
                        {
                            discountTotal = 265 * creditsRemain;
                            _logger.Info("COC create partial discount  " + row.Discount.idDiscount + " on " + row.idOrder + " found "
                                + creditsRemain + " against " + row.RegistrationType.CreditCost);
                        }
                        else
                        {
                            discountTotal = 0;
                            _logger.Info("COC failed to create discount  " + row.Discount.idDiscount + " on " + row.idOrder + " found "
                                + creditsRemain + " against " + row.RegistrationType.CreditCost);
                            row.Discount = null;
                        }
                    }
                }
                else
                {
                    discountTotal = row.RowPrice * row.Discount.PercentOff / 100;
                    _logger.Info("COC found non WPS discount  " + row.Discount.idDiscount + " on " + row.idOrder + " found "
                            + creditsRemain + " against " + row.RegistrationType.CreditCost);
                }

            }
            else if (row.Discount != null && row.Discount.FlatOff != 0.0M)
            {
                discountTotal = row.Discount.FlatOff;
                _logger.Info("COC found non flatOff discount " + row.Discount.idDiscount + " on " + row.idOrder + " found " + row.Discount.FlatOff);
            }

            if (discountTotal >= row.RowPrice)
            {
                discountTotal = row.RowPrice;
            }
            else
            {
                ProcessPartialDiscount(row, discountTotal);
            }

            row.RowPrice -= discountTotal;
            pricesAndDiscounts.TotalDiscount = discountTotal;
            pricesAndDiscounts.TotalCostOfOptions = totalOptionsPrice;

            pricesAndDiscounts.TaxAmount = 0;

            if (order.BillingState == "WI"
                && !row.RegistrationType.OptionLabel.StartsWith("Live Plus Five")
                && row.RowPrice > 0
                )
            {

                pricesAndDiscounts.TaxAmount = Math.Round(row.RowPrice * Convert.ToDecimal(.055), 2);
                _logger.Info("COC found tax on: " + row.idOrder + " found " + pricesAndDiscounts.TaxAmount);
            }


            //Calculate order total
            order.Total = row.RowPrice + pricesAndDiscounts.TaxAmount;
            pricesAndDiscounts.Discount = row.Discount;
            pricesAndDiscounts.TotalOrderPrice = order.Total;

            //var discountJsonString =
            //    new JObject(
            //        new JProperty("Discount", discountJson)
            //        );
            //var rowJson =
            //    new JObject(
            //           new JProperty("Row", JsonConvert.SerializeObject(row, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));


            //_logger.Info(discountJsonString.ToString());
            //_logger.Info(rowJson.ToString());

            return pricesAndDiscounts;
        }

        private OrderRow ProcessPartialDiscount(OrderRow row, decimal discountTotal)
        {
            return null;
        }

        public decimal DiscountCreditUnitCost { get; private set; }

        public void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false,
            Uri url = null)
        {

            //override userCreatedInCart to remedy express checkout problem where existing users
            // are being prompted to confirm password.

            var doesUserExist = _webUserRepository.GetWebUserFullname(order.BillingEmail);
            // GetOrdersByUserId(order.idUser).Where(o => o.idOrder  < order.idOrder && (o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.Submitted))

            if (doesUserExist != null)
                userCreatedInCart = false;

            string addPasswordUrl = string.Empty;
            var idOrderToShow = order.idOrderLegacy;
            if (order.idOrderLegacy == 0)
            {
                idOrderToShow = order.idOrder;
            }
            var orderSubmittedViewModel = new ConfirmOrderMessage
            {
                AddPasswordUrl = string.Empty,
                ConfirmChangeEmailUrl = string.Empty,
                Details = order.NotificationStorage,
                idOrder = idOrderToShow,
                Order = order,
                OrderGenesis =
                    userCreatedInCart ? OrderGenesis.CreatedViaCartByNewUser : OrderGenesis.CreatedViaCartByExistingUser,
                UserCreatedInCart = userCreatedInCart,
                UserCreatedOnImport = false
            };

            if (!ReferenceEquals(null, url))
            {
                var baseUri = new Uri(string.Concat(url.Scheme, @"://", url.Authority), UriKind.Absolute);
                addPasswordUrl = new Uri(
                    baseUri,
                    string.Concat(@"acc/apwd/", order.idOrder)
                    ).ToString();
            }

            AddEvent(new OrderSubmittedEvent<ConfirmOrderMessage>
            {
                Details = order.NotificationStorage,
                EventObject = orderSubmittedViewModel,
                RelativePath = addPasswordUrl,
                ResendEvent = resending
            });

            foreach (var evt in GetEvents().OfType<OrderSubmittedEvent<ConfirmOrderMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            int rowsUpdated = _orderRepository.SaveChanges();

            Clear(); // need to clear at this point, otherwise the OrderSubmittedEvent will be fired again when 

            if (!ReferenceEquals(order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation, null)
                && order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count != 0)
            {
                foreach (var addLoc in order.OrderRows
                    .Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                {
                    FireOrderSubmittedAdditionalLocationEvent(order, addLoc.Email, resending);
                }
            }
        }

        public void FireAdminEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients, bool resending = false)
        {
            AddEvent(new AdminEmailSendShippedOrderEvent<Order> { EventObject = order, Recipients = recipients, ResendEvent = resending });


            foreach (var evt in GetEvents().OfType<AdminEmailSendShippedOrderEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public void FireAdminEmailConnectionInfoHandler(Order order, IEnumerable<string> recipients)
        {
            AddEvent(new AdminEmailConnectionInfoEvent<Order> { EventObject = order, Recipients = recipients });


            foreach (var evt in GetEvents().OfType<AdminEmailConnectionInfoEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int rowsUpdated = _orderRepository.SaveChanges();
        }

        public void FireAdminEmailRecordingPostedHandler(Order order, IEnumerable<string> recipients)
        {
            AddEvent(new AdminEmailRecordingPostedEvent<Order> { EventObject = order, Recipients = recipients });


            foreach (var evt in GetEvents().OfType<AdminEmailRecordingPostedEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int rowsUpdated = _orderRepository.SaveChanges();
        }

        public void FireOrderSubmittedAdditionalLocationEvent(Order order, string address, bool resending)
        {
            _logger.Info("Adding Event for Order with Additional Location {0} - {1}", order.idOrder, address);

            //var orderSubmittedAdditionalLocationViewModel = new OrderSubmittedAdditionalLocationViewModel
            //{
            //    ConfirmChangeEmailUrl = string.Empty,
            //    Order = order,
            //    UserCreatedOnImport = false,
            //    NotifyAddress = address
            //};

            var additionalLocationOrderDetails = new AdditionalLocationOrderDetailsMessage
            {
                ConfirmChangeEmailUrl = string.Empty,
                idOrder = order.idOrder,
                Order = order,
                UserCreatedOnImport = false,
                NotifyAddress = address,

            };

            //var relativePath = Path.Combine(@"App_Data\Notifications", string.Format("OrderNotificationAddLoc-{0}{1}", DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat), ".htm"));

            AddEvent(new OrderSubmittedAdditionalLocationEvent<AdditionalLocationOrderDetailsMessage>
            {
                Details = order.NotificationStorage,
                EventObject = additionalLocationOrderDetails,
                ResendEvent = resending
            });

            //_logger.Info("Persisted Email for Order w/AddLoc {0}-{1} :{2}", order.idOrder, address, relativePath);

            foreach (var evt in GetEvents().OfType<OrderSubmittedAdditionalLocationEvent<AdditionalLocationOrderDetailsMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            //int rowsUpdated = _orderRepository.SaveChanges();
        }

        public void FireSendRecordingIsPostedEvent(IList<Order> orders)
        {

            foreach (var order in orders)
            {
                Double[] i = _webinarRepository.GetCostOfUpgrades(order.OrderRows.Single().idRegType);



                Double basePrice = i[0];
                Double cost6 = i[1];
                Double costCD = i[2];

                Discount discount = null;

                if (ReferenceEquals(order.OrderRows.Where(o => o.RowStatus == OrderRowStatus.Active), null))
                {
                    discount =
                        order.OrderRows.Where(o => o.RowStatus == OrderRowStatus.Active).SingleOrDefault().Discount;
                }
                else
                {
                    discount = GetDiscountById(order.idOrder);
                }

                string orderSum = OrderSummaryBuilder(order, discount);
                _logger.Info("SendRecordingIsPosted: " + order.idOrder);
                var postEventPublishModel = new PostEventPublishModel()
                {
                    Order = order,
                    CostFor6month = (Convert.ToDecimal(cost6) - Convert.ToDecimal(basePrice)).ToString().Replace(".00", ""),
                    CostForCD = (Convert.ToDecimal(costCD) - Convert.ToDecimal(basePrice)).ToString().Replace(".00", ""),
                    //ExpiryDate = CalculatePostEventMaterialsAccessExpiry(order).ToShortDateString(),
                    OrderSummaryString = orderSum
                };
                AddEvent(new SendRecordingPostedEvent<PostEventPublishModel>
                {
                    EventObject = postEventPublishModel,
                    ResendEvent = false
                });
            }

            foreach (var evt in GetEvents().OfType<SendRecordingPostedEvent<PostEventPublishModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int numRows = _orderRepository.SaveChanges();
        }

        private string OrderSummaryBuilder(Order order, Discount discount)
        {
            var row = from orderRow in order.OrderRows
                      where orderRow.RowStatus == OrderRowStatus.Active
                      select orderRow;
            var myRow = row.Single();

            //var mySub = myRow.Order.d

            var webinar = myRow.Webinar;
            var OptionLabel = GetRegTypeOfOrderRow(myRow.idRegType).OptionLabel;

            var sb = new StringBuilder();
            if (webinar.Title.StartsWith("Compliance Perspectives"))
            {

                //var subStarted = "Valid from: " + sub

                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Title:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");
                sb.Append("Compliance Perspectives - " + webinar.Date.ToString("MMMM yyyy"));
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");

                //presenter cell
                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Presenter:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>Carl Pry");
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");

                //
                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Subscription:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");
                sb.Append(myRow.RegistrationType.OptionLabel.Replace(" Subscription", "").Replace("-", " "));
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                if (!ReferenceEquals(discount, null))
                {
                    string subscriptionSpan = "";
                    sb.Append("<tr>");
                    if (!ReferenceEquals(discount.DateValidFrom, null))
                    {
                        subscriptionSpan = discount.DateValidFrom.ToString("MMM-yy") + " until " + discount.DateValidTo.ToString("MMM-yy");

                        sb.Append(
                            "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                        sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                        sb.Append("            Term:");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                        sb.Append(
                            "    <td width='350px' style='text-align: left; color: red; background-color: #B4D1EC; padding-left: 6px;'>");
                        sb.Append(
                            "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                        sb.Append("            <b>");
                        sb.Append(subscriptionSpan);
                        sb.Append("            </b>");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                    }
                    sb.Append("</tr>");
                }

                //insert CP-specific wording for subsrip stats.



                //

            }
            else
            {

                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Title:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");
                sb.Append(webinar.Title);
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Date:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");
                sb.Append(webinar.Date.ToShortDateString());
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Attendance Type:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");
                sb.Append(OptionLabel);
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                //if (!ReferenceEquals(myRow.Discount, null))
                //{
                //    decimal amountToReduce;
                //    sb.Append("<tr>");
                //    if (!ReferenceEquals(myRow.Discount.FlatOff, null))
                //    {
                //        amountToReduce = Convert.ToDecimal(myRow.Order.Total) - (myRow.Discount.FlatOff);
                //        sb.Append(
                //            "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                //        sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                //        sb.Append("            Amt. of Discount:");
                //        sb.Append("        </span>");
                //        sb.Append("    </td>");
                //        sb.Append(
                //            "    <td width='350px' style='text-align: left; color: red; background-color: #B4D1EC; padding-left: 6px;'>");
                //        sb.Append(
                //            "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                //        sb.Append("            <b>$");
                //        sb.Append(amountToReduce.ToString().Replace(".00", ""));
                //        sb.Append("            </b>");
                //        sb.Append("        </span>");
                //        sb.Append("    </td>");
                //    }
                //    if (!ReferenceEquals(myRow.Discount.PercentOff, null))
                //    {
                //        amountToReduce = (myRow.Discount.PercentOff * 100) /
                //                         Convert.ToDecimal(string.Format("{0:0.00}", myRow.UnitPrice));

                //        sb.Append(
                //            "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                //        sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                //        sb.Append("            Amt. of Discount:");
                //        sb.Append("        </span>");
                //        sb.Append("    </td>");
                //        sb.Append(
                //            "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                //        sb.Append(
                //            "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                //        sb.Append("            <b>$");
                //        sb.Append(amountToReduce.ToString().Replace(".00", ""));
                //        sb.Append("            </b>");
                //        sb.Append("        </span>");
                //        sb.Append("    </td>");
                //    }
                //    sb.Append("</tr>");
                //}

                //sb.Append("<tr>");
                //sb.Append(
                //    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                //sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                //sb.Append("            Cost:");
                //sb.Append("        </span>");
                //sb.Append("    </td>");
                //sb.Append(
                //    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                //sb.Append(
                //    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                //sb.Append("            <b>$");
                //sb.Append(myRow.Order.Total.ToString().Replace(".00", ""));
                //sb.Append("            </b>");
                //sb.Append("        </span>");
                //sb.Append("    </td>");
                //sb.Append("</tr>");
                //sb.Append("<tr>");
                //sb.Append(
                //    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                //sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                //sb.Append("            OnDemand Access Expires:");
                //sb.Append("        </span>");
                //sb.Append("    </td>");
                //sb.Append(
                //    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                //sb.Append("");
                //sb.Append(
                //    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                //sb.Append("            <b>");

                //sb.Append(CalculatePostEventMaterialsAccessExpiry(order).ToShortDateString());
                //sb.Append("            </b>");
                //sb.Append("        </span>");
                //sb.Append("    </td>");
                //sb.Append("</tr>");

            }
            return sb.ToString();

        }


        public void FireSendPerDayPromoEvent(WebinarPromoViewModel webinarPromoViewModel)
        {
            AddEvent(new SendPerDayPromoEvent<WebinarPromoViewModel> { EventObject = webinarPromoViewModel });

            foreach (var evt in GetEvents().OfType<SendPerDayPromoEvent<WebinarPromoViewModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int rowsUpdated = _orderRepository.SaveChanges();
        }

        public void FireSendPerWeekPromoEvent(IList<Affiliate> affiliates, Webinar webinar)
        {
            throw new NotImplementedException();
        }

        public void FireSendReminderNotificationEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendReminderEvent<Order> { EventObject = order, Details = order.NotificationStorage });
            }

            foreach (var evt in GetEvents().OfType<SendReminderEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int numRows = _orderRepository.SaveChanges();
        }

        public void FireSendConnectionInfoNotificationEvent(IList<Order> orders, bool resending)
        {

            var idWebinar = orders[0].OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.idWebinar;

            var citrixRegs = GetCitrixRegistrantsByWebinar(idWebinar);


            foreach (var order in orders)
            {
                GenerateRegistrantKey(order);
                AddEvent(new SendConnectionInfoEvent<Order> { EventObject = order, ResendEvent = resending, Details = order.NotificationStorage });
            //see SendConnectionInfoHandler for handling implementation
            }

            foreach (var evt in GetEvents().OfType<SendConnectionInfoEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int numRows = _orderRepository.SaveChanges();
        }

        public void FireSendOrderShippedNotificationEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendShippedOrderEvent<Order> { EventObject = order });
            }


            foreach (var evt in GetEvents().OfType<SendShippedOrderEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int numRows = _orderRepository.SaveChanges();
        }

        public void FireSendRecordingPostedNotificationEvent(IList<Order> orders)
        {
            var title = orders[0].OrderRows.SingleOrDefault().Webinar.Title;

            _logger.Info("Begins RecordingPostedNotification for {1} with {0} orders.", orders.Count, title);
            var x = 0;
            foreach (var order in orders)
            {
                _logger.Info("RecordingPostedNotification {0} of {1} sent to {2} for {3}.", x, orders.Count, order.BillingEmail, title);

                var postEventPublishModel = new PostEventPublishModel()
                {
                    Order = order
                };
                AddEvent(new SendRecordingPostedEvent<PostEventPublishModel>
                {
                    EventObject = postEventPublishModel
                });
            }

            foreach (var evt in GetEvents().OfType<SendRecordingPostedEvent<PostEventPublishModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            int numRows = _orderRepository.SaveChanges();
        }

        public void FireSendWeeklyInvoiceEvent(SendWeeklyInvoiceViewModel weeklyInvoiceViewModel)
        {
            AddEvent(new SendWeeklyInvoiceEvent<SendWeeklyInvoiceViewModel> { EventObject = weeklyInvoiceViewModel });

            foreach (var evt in GetEvents().OfType<SendWeeklyInvoiceEvent<SendWeeklyInvoiceViewModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            // int rowsUpdated = _orderRepository.SaveChanges(); // TODO: zzz ALS confirm we can remove, then remove
        }

        public void FireOrderSubmittedMultiEvent(string toEmail, string subject, string body)
        {
            var orderSubmittedMultiMessage = new OrderSubmittedMultiMessage()
            {
                Recipients = toEmail.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                Subject = subject,
                Body = body,
            };

            AddEvent(new OrderSubmittedMultiEvent<OrderSubmittedMultiMessage> { EventObject = orderSubmittedMultiMessage });

            foreach (var evt in GetEvents().OfType<OrderSubmittedMultiEvent<OrderSubmittedMultiMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            // int rowsUpdated = _orderRepository.SaveChanges(); // TODO: zzz ALS confirm we can remove, then remove
        }

        public string UpdateOrderChanges(Order newOrder, ref PricesAndDiscounts pricesAndDiscounts)
        {
            try
            {

                var originalOrder = GetOrderById(newOrder.idOrder);
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                var additionalLocationsPricing = newOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.AdditionalLocationPrice;
                //var additionalLocationsPricing = GetAdditionalLocationsPricing(
                //    newOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar
                //    );

                string preSaveValues = dataOperations.GetPreSaveValues(newOrder.idOrder);
                string pRowPrice = preSaveValues.Split(',')[0];
                string pOrderStatus = preSaveValues.Split(',')[1];
                string pDiscount_idDiscount = preSaveValues.Split(',')[2];
                string pAddLocsPrice = preSaveValues.Split(',')[3];
                string regTypeShortened =
                    preSaveValues.Split(',')[4].Replace(" Package", "")
                        .Replace("Live Plus Six", "Live+6")
                        .Replace(" and Hardcopy Handouts", "")
                        .Replace(" Recording Only", "")
                        .Replace(" Plus Five", "+5");
                int idRegTypeOfOrg = Convert.ToInt32(preSaveValues.Split(',')[5]);

                pricesAndDiscounts = CalculateOrderCost(newOrder, additionalLocationsPricing);

                if (newOrder.OrderRows != null)
                {
                    if (
                        newOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.idRegType != idRegTypeOfOrg)
                    {
                        // tracked by sproc: EXEC OrderIsUpdated
                        _logger.Info("UpdateOrderChanges: {0} went from: {1} - {2} to: {3} - {4}", newOrder.idOrder,
                            regTypeShortened, pRowPrice,
                            newOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                .RegistrationType.OptionLabel,
                            newOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).RowPrice);
                        try
                        {
                            if (!string.IsNullOrEmpty(newOrder.InvoiceDetail) && newOrder.InvoiceDetail.Contains("OrderIsInvoiced"))
                            {
                                _logger.Warn("Invoiced Order is updated: " + newOrder.idOrder);

                                var toJson = JObject.Parse(newOrder.InvoiceDetail);
                                var orgInvoiceDetails =
                                    toJson.Properties().FirstOrDefault(p => p.Name.StartsWith("OrderIsInvoiced"));

                                if (orgInvoiceDetails != null)
                                {
                                    var row =
                                        newOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                    //
                                    Debug.Assert(row != null, "row != null");
                                    row.Royalty = row.RowPrice * (decimal)orgInvoiceDetails.First()["PercentPaid"];

                                    StringBuilder sb = new StringBuilder();

                                    sb.Append(newOrder.idOrder + " was " + regTypeShortened);
                                    sb.Append(" ($" + pRowPrice.ToString().Replace(".0000", "").Replace(".00", "") +
                                              ") on InvoiceID " + orgInvoiceDetails.First()["InvoiceId"] +
                                              " but changed to " +
                                              row.RegistrationType.OptionLabelShort + " (" +
                                              row.RowPrice.ToString("C").Replace(".00", "") + ") on " + TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");

                                    decimal adustmentAmount;
                                    var adjustmentDirection = "Royalty is increased";

                                    if ((decimal)orgInvoiceDetails.First()["AmountOfRoyalty"] < row.Royalty)
                                    {
                                        adustmentAmount = row.Royalty -
                                                          (decimal)orgInvoiceDetails.First()["AmountOfRoyalty"];
                                        sb.Append(adjustmentDirection + " by " +
                                                  adustmentAmount.ToString("C").Replace(".00", ""));

                                    }
                                    else
                                    {
                                        adjustmentDirection = "Royalty is decreased";
                                        adustmentAmount = row.Royalty -
                                                          (decimal)orgInvoiceDetails.First()["AmountOfRoyalty"];
                                        sb.Append(adjustmentDirection + " by " +
                                                  (-adustmentAmount).ToString("C").Replace(".00", ""));
                                    }


                                    var newJson4Invoice = new JProperty(
                                        "ChangedOrderNeedsNewInvoice",
                                        new JObject(
                                            new JProperty("OriginalInvoice", orgInvoiceDetails.First()["InvoiceId"].ToString()),
                                            new JProperty("OriginalDateOfInvoice", orgInvoiceDetails.First()["DateOfInvoice"].ToString()),
                                            new JProperty("OriginalTotal", orgInvoiceDetails.First()["AmountOfOrder"].ToString()),
                                            new JProperty("OriginalPercentPaid", orgInvoiceDetails.First()["PercentPaid"].ToString()),
                                            new JProperty("OriginalRoyaltyPaid", orgInvoiceDetails.First()["AmountOfRoyalty"].ToString()),
                                            new JProperty("OriginalAffiliate", orgInvoiceDetails.First()["Affiliate"].ToString()),
                                            new JProperty(adjustmentDirection, adustmentAmount),
                                            new JProperty("DateOfChange", TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                                            new JProperty("Message", sb.ToString())
                                            ));


                                    newOrder.InvoiceDetail = JsonHelpers.ReplaceJsonWithStoredField(
                                        newOrder.InvoiceDetail, newJson4Invoice, "OrderIsInvoiced");

                                }
                                else
                                {
                                    orgInvoiceDetails =
                                        toJson.Properties()
                                            .FirstOrDefault(p => p.Name.StartsWith("ChangedOrderNeedsNewInvoice"));
                                    //properties saved when updated:
                                    //{ChangedOrderNeedsNewInvoice:{
                                    //OriginalInvoice
                                    //OriginalDateOfInvoice
                                    //OriginalTotal
                                    //OriginalPercentPaid
                                    //OriginalRoyaltyPaid
                                    //OriginalAffiliate
                                    //Royalty is increased
                                    //DateOfChange
                                    _logger.Warn("ChangedOrderNeedsNewInvoice: " + orgInvoiceDetails);

                                    Debug.Assert(orgInvoiceDetails != null, "orgInvoiceDetails != null");
                                    string[] tokens = orgInvoiceDetails.First()["Message"].ToString().Split(' ');
                                    string retVal = tokens[0] + " " + tokens[4];

                                    pRowPrice = orgInvoiceDetails.First()["OriginalTotal"].ToString();
                                    //string pOrderStatus = preSaveValues.Split(',')[1];
                                    //string pDiscount_idDiscount = ""; //TODO account for subscription orders;
                                    //string pAddLocsPrice = ""; //TODO: account for addLoc prices
                                    regTypeShortened = tokens[3];
                                    //string regTypeShortened = orgInvoiceDetails.First()["Message"].ToString().Substring(19,  retVal.Length);

                                    var row =
                                        newOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                                    //
                                    row.Royalty = row.RowPrice *
                                                  (decimal)orgInvoiceDetails.First()["OriginalPercentPaid"];

                                    StringBuilder sb = new StringBuilder();

                                    sb.Append(newOrder.idOrder + " was " + regTypeShortened);
                                    sb.Append(" ($" + pRowPrice.ToString().Replace(".0000", "").Replace(".00", "") +
                                              ") on InvoiceID " + orgInvoiceDetails.First()["InvoiceId"] +
                                              " but changed to " +
                                              row.RegistrationType.OptionLabelShort + " (" +
                                              row.RowPrice.ToString("C").Replace(".00", "") + ") on " + TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");


                                    decimal adustmentAmount;
                                    var adjustmentDirection = "Royalty is increased";

                                    if ((decimal)orgInvoiceDetails.First()["OriginalRoyaltyPaid"] < row.Royalty)
                                    {
                                        adustmentAmount = row.Royalty -
                                                          (decimal)orgInvoiceDetails.First()["OriginalRoyaltyPaid"];
                                        sb.Append(adjustmentDirection + " by " +
                                                  adustmentAmount.ToString("C").Replace(".00", ""));

                                    }
                                    else
                                    {

                                        adjustmentDirection = "Royalty is decreased";
                                        adustmentAmount = row.Royalty -
                                                          (decimal)orgInvoiceDetails.First()["OriginalRoyaltyPaid"];
                                        sb.Append(adjustmentDirection + " by " +
                                                  (adustmentAmount).ToString("C").Replace(".00", ""));
                                    }

                                    var newJson4Invoice = new JProperty(
                                        "ChangedOrderNeedsNewInvoice",
                                        new JObject(
                                            new JProperty("OriginalInvoice", orgInvoiceDetails.First()["OriginalInvoice"].ToString()),
                                            new JProperty("OriginalDateOfInvoice", orgInvoiceDetails.First()["OriginalDateOfInvoice"].ToString()),
                                            new JProperty("OriginalTotal", orgInvoiceDetails.First()["OriginalTotal"].ToString()),
                                            new JProperty("OriginalPercentPaid", orgInvoiceDetails.First()["OriginalPercentPaid"].ToString()),
                                            new JProperty("OriginalRoyaltyPaid", orgInvoiceDetails.First()["OriginalRoyaltyPaid"].ToString()),
                                            new JProperty("OriginalAffiliate", orgInvoiceDetails.First()["OriginalAffiliate"].ToString()),
                                            new JProperty(adjustmentDirection, adustmentAmount),
                                            new JProperty("DateOfChange", TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
                                            new JProperty("Message", sb.ToString())
                                            ));


                                    newOrder.InvoiceDetail = JsonHelpers.ReplaceJsonWithStoredField(
                                        newOrder.InvoiceDetail, newJson4Invoice, "ChangedOrderNeedsNewInvoice");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.FatalException("UpdateOrderChanged Json Merge: ", ex);
                        }
                    }
                }

                _orderRepository.SaveOrderChanges(newOrder, (int)newOrder.OrderStatus);

                Clear();
                return "success";
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("SaveOrderChanges method: {0} - {1}", newOrder.idOrder, exception.Message), exception);
            }
            return "failed";
        }

        public void UpdateOrderWithUserEmail(int orderId, string email)
        {
            var order = _orderRepository.FindById(orderId);
            order.BillingEmail = email;

            var updatedOrder = _orderRepository.SaveOrderChanges(order, null);
        }

        public void UpdateOrderWithUserId(int orderId, int userId)
        {
            var user = _webUserRepository.FindByIdLoaded(userId);
            var order = _orderRepository.FindById(orderId);
            order.AuditInfo = "anon user is updated with " + user.email + Environment.NewLine + order.AuditInfo;
            order.idUser = userId;

            var billingAddress = user.Addresses.Where(a => a.AddressType == DomainConstants.BillingAddress).SingleOrDefault();
            var shippingAddress = user.Addresses.Where(a => a.AddressType == DomainConstants.ShippingAddress).SingleOrDefault();

            if (ReferenceEquals(billingAddress, null))
            {
                billingAddress = new Address
                {
                    AddressType = "Billing",
                    Country = "USA",
                    City = "-",
                    StreetAddress = "-",
                    State = "-",
                    StreetAddress2 = "-",
                    Name = "-",
                    Phone = "555-555-5555",
                    Zip = "00000",
                    idUser = userId
                };
            }
            if (ReferenceEquals(shippingAddress, null))
            {
                shippingAddress = new Address
                {
                    AddressType = "Shipping",
                    Country = "USA",
                    City = "-",
                    StreetAddress = "-",
                    State = "-",
                    StreetAddress2 = "-",
                    Name = "-",
                    Phone = "555-555-5555",
                    Zip = "00000",
                    idUser = userId
                };
            }

            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.Institution = user.Institution.InstitutionName;
            order.BillingPhone = billingAddress.Phone ?? "";
            order.BillingEmail = user.email;
            order.BillingAddress = billingAddress.StreetAddress ?? "";
            order.BillingAddress2 = billingAddress.StreetAddress2 ?? "";
            order.BillingState = billingAddress.State ?? "";
            order.BillingCity = billingAddress.City ?? "";
            order.BillingZip = billingAddress.Zip ?? "";

            order.ShippingFirstName = user.FirstName;
            order.ShippingLastName = user.LastName;
            order.ShippingPhone = shippingAddress.Phone ?? "";
            order.ShippingAddress = shippingAddress.StreetAddress ?? "";
            order.ShippingAddress2 = shippingAddress.StreetAddress2 ?? "";
            order.ShippingState = shippingAddress.State ?? "";
            order.ShippingCity = shippingAddress.City ?? "";
            order.ShippingZip = shippingAddress.Zip ?? "";

            order.Institution = user.Institution.InstitutionName;

            _orderRepository.SaveOrderChanges(order, null);
        }

        public void RemoveAdditionalLocationsForOrder(int idOrderRow)
        {
            try
            {
                _additionalLocationsRepository.DeleteAdditionalLocationsByOrderRowId(idOrderRow);

                var orderRow = _orderRepository.GetOrderRowById(idOrderRow);
                var additionalLocationsPricing = GetAdditionalLocationsPricing(orderRow.idWebinar);

                var totalOptionsDeletedCost = additionalLocationsPricing * orderRow.AdditionalLocation.Count;
                SetTotalPrice(totalOptionsDeletedCost, orderRow);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("RemoveAdditionalLocationsForOrder | idOrderRow={0}", idOrderRow), exception);
            }
        }

        private void SetTotalPrice(decimal newTotalPrice, OrderRow orderRow)
        {
            orderRow.RowPrice = newTotalPrice;
            orderRow.Order.Total = newTotalPrice;
            _orderRepository.SaveOrderChanges(orderRow.Order, null);
        }

        public Discount GetDiscountByCode(string discount)
        {
            var myDiscount = _orderRepository.FindDiscountByCode(discount);
            return myDiscount;
        }

        public Discount GetDiscountByOrderId(int idOrder)
        {
            var myDiscount = _orderRepository.GetDiscountByOrderId(idOrder);
            return myDiscount;
        }

        //public Discount GetDiscountById(int discount)
        //{
        //    var myDiscount = _orderRepository.FindDiscountById(discount);
        //    return myDiscount;
        //}

        public decimal GetAdditionalLocationsPricing(int idWebinar)
        {
            return _webinarRepository.FindById(idWebinar).AdditionalLocationPrice;
        }



        public void UpdateOrderByAdmin(Order order)
        {
            var updatedOrder = _orderRepository.SaveOrderChanges(order, 0);
        }


        public PostEventClaim FindPostEventClaimByOnDemandCode(Order order)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            return dataOperations.FindPostEventClaimByOnDemandCode(order);

        }

        public IList<PostEventClaim> FindAllPostEventClaims()
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            return dataOperations.FindAllPostEventClaims();

        }

        public Webinar GetWebinarById(int webinarId)
        {
            return _webinarRepository.GetWebinarByIdIncludingAllWebinarsByPresenter(webinarId);
        }

        public void UpdateUserDetails(WebUser user,
            string firstName,
            string lastName,
                string email,
                string institution,
                Address billingAddress,
                Address shippingAddress
                )
        {

            var orders = _orderRepository.GetOrdersByUserId(user.idUser);

            foreach (var order in orders)
            {
                var changedVals = "";
                if (order.FirstName != firstName) { changedVals += " First Name: " + order.FirstName + "  to " + firstName; }
                if (order.LastName != lastName) { changedVals += " Last Name: " + order.LastName + "  to " + lastName; }
                if (order.BillingEmail != email) { changedVals += " Email: " + order.BillingEmail + "  to " + email; }
                if (order.Institution != institution) { changedVals += " Institution: " + order.Institution + "  to " + institution; }
                if (order.BillingAddress != billingAddress.StreetAddress) { changedVals += " Street Address: " + order.BillingAddress + "  to " + billingAddress.StreetAddress; }
                if (order.BillingAddress2 != billingAddress.StreetAddress2) { changedVals += " Street Address2: " + order.BillingAddress2 + "  to " + billingAddress.StreetAddress2; }
                if (order.BillingCity != billingAddress.City) { changedVals += " City: " + order.BillingCity + "  to " + billingAddress.City; }
                if (order.BillingState != billingAddress.State) { changedVals += " State: " + order.BillingState + "  to " + billingAddress.State; }
                if (order.BillingZip != billingAddress.Zip) { changedVals += " Zip: " + order.BillingZip + "  to " + billingAddress.Zip; }

                if (order.ShippingAddress != shippingAddress.StreetAddress) { changedVals += " Shipping Address: " + order.ShippingAddress + "  to " + shippingAddress.StreetAddress; }
                if (order.ShippingAddress2 != shippingAddress.StreetAddress2) { changedVals += " Shipping Address2: " + order.ShippingAddress2 + "  to " + shippingAddress.StreetAddress2; }
                if (order.ShippingCity != shippingAddress.City) { changedVals += " Shipping City: " + order.ShippingCity + "  to " + shippingAddress.City; }
                if (order.ShippingState != shippingAddress.State) { changedVals += " Shipping State: " + order.ShippingState + "  to " + shippingAddress.State; }
                if (order.ShippingZip != shippingAddress.Zip) { changedVals += " Shipping Zip: " + order.ShippingZip + "  to " + shippingAddress.Zip; }


                order.FirstName = firstName;
                order.LastName = lastName;
                order.BillingEmail = email;
                order.Institution = institution;
                order.BillingAddress = billingAddress.StreetAddress;
                order.BillingAddress2 = billingAddress.StreetAddress2;
                order.BillingCity = billingAddress.City;
                order.BillingState = billingAddress.State;
                order.BillingZip = billingAddress.Zip;

                order.ShippingAddress = shippingAddress.StreetAddress;
                order.ShippingAddress2 = shippingAddress.StreetAddress2;
                order.ShippingCity = shippingAddress.City;
                order.ShippingState = shippingAddress.State;
                order.ShippingZip = shippingAddress.Zip;
                order.UserComments += "{'ContactInfoUpdated': '" + changedVals + "'}";
                _logger.Info("idOrder {0} user info updated:  {1}", order.idOrder, changedVals);

                SaveChanges();
            }

        }

        public string GetOnDemandClaimByCode(string onDemandCode)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var retClaim = dataOperations.GetOnDemandClaimByCode(onDemandCode);
            return retClaim;

        }

        public string GetOnDemandClaimById(int idOrder)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var retClaim = dataOperations.GetOnDemandClaimById(idOrder);
            return retClaim;
        }

        public Order GetOrderByOnDemandClaim(string onDemandCode)
        {
            var order = _orderRepository.GetOrderByIdByOnDemandCode(onDemandCode);
            return order;
        }

        public void UpdateDiscountDetails(Discount discount)
        {
            _discountRepository.SaveChanges(discount);
        }

        public void UpdateShippingAddressDetails(Address shippingAddress, int idUser)
        {
            _orderRepository.UpdateShippingAddressDetails(shippingAddress, idUser);
        }


        public RegType GetRegTypeByLabel(string regType, int idWebinar)
        {
            return _regTypeRepository.GetRegTypeByLabel(regType, idWebinar);
        }

        public bool OnDemandCodeIsUnique(string onDemandCode)
        {
            return _orderRepository.OnDemandCodeIsUnique(onDemandCode);
        }

        public IList<int> GetV3OrdersIdsByWebinar(int idWebinar)
        {
            return _webinarRepository.GetV3OrdersIdsByWebinar(idWebinar);
        }

        public List<Order> GetOrdersByWebinar(int idWebinar)
        {
            return _webinarRepository.GetOrdersByWebinar(idWebinar).ToList();
        }

        public List<Order> GetOrdersByWebinarForInvoice(int idWebinar)
        {

            return _webinarRepository.GetOrdersByWebinarForInvoice(idWebinar).ToList();

        }

        public void RestoreToDiscount(int newOrderRowId)
        {
            _logger.Fatal("logs the restoration of discount credit when addLocation is deleted.");
        }

        public void RemoveFromDiscount(int newOrderRowId)
        {
            _logger.Fatal("logs the decrement of discount credit when addLocation is added.");
        }

        public IList<Order> GetOrdersByDomain(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public IList<Order> GetOrdersByDiscount(int idDiscount)
        {
            return _orderRepository.GetOrdersByDiscount(idDiscount);
        }


        public IList<Order> GetV3OrdersByOnDemandClaim()
        {
            return _orderRepository.GetV3OrdersByOnDemandClaim();
        }

        public Discount GetDiscountByUser(WebUser currentUser)
        {
            var myDiscount = _orderRepository.FindDiscountByUser(currentUser);
            return myDiscount;
        }

        public Order GenerateRegistrantKey(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var regKeyResponse = string.Empty;

            var registrant = new Registrant();

            try
            {
                //Debug.Assert(!string.IsNullOrWhiteSpace(row.CitrixJoinUrl), "CitrixJoinUrl should always be null or empty before this method is called as a pre-condition.");
                if (row.TtsJoinUrl != null && row.RegistrationType.ShowLiveNotifications.TrimEnd()
                        .Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {

                    registrant = CreateRegistrantKey(
                        order.FirstName ?? " ",
                        order.LastName ?? " ",
                        order.BillingEmail,
                        row.Webinar.idWebinar,
                        row.Webinar.WebinarKey
                    );

                    row.CitrixJoinUrl = registrant.joinUrl;
                    row.RegistrantKey = registrant.registrantKey.ToString();

                    if (row.AdditionalLocation != null)
                    {
                        // This branch gets key for main Additional Locations
                        foreach (var addLoc in row.AdditionalLocation)
                        {
                            var _regKeyResponse = CreateRegistrantKey(
                                "c/o " + order.FirstName,
                                order.LastName ?? " ",
                                addLoc.Email,
                                row.Webinar.idWebinar,
                                row.Webinar.WebinarKey
                            );
                            addLoc.RegistrantKey = _regKeyResponse.registrantKey.ToString();
                            addLoc.JoinURL = _regKeyResponse.joinUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("GenerateRegistrantKey: ", ex);
            }

            if (string.IsNullOrWhiteSpace(regKeyResponse))
            {
                _logger.FatalException("The Registration Key creation failed.", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response."));
            }
            else
            {
                _logger.Info("GenerateRegistrantKey for " + order.idOrder + " = " + regKeyResponse);

            }
            return order;
        }

        public int GetNumberOfOrdersPerWebinar(int id)
        {
            return _orderRepository.GetNumberOfOrdersPerWebinar(id);
        }

        public void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation)
        {
            _orderRepository.RemoveAndDeleteAdditionalLocation(deletedAdditionalLocation);
        }


        public DateTime CalculatePostEventMaterialsAccessExpiry(OrderRow row)
        {
            if (row == null) throw new ArgumentNullException("orderrow_CalcPostEvent");

            try
            {
                var orderDate = DateTime.Now;
                if (row.Order != null)
                {
                    orderDate = row.Order.OrderDate;
                }

                var regType = GetRegTypeOfOrderRow(row.idRegType);

                //establish order date as starting point
                DateTime expryDate = orderDate.AddMonths(6);

                var webinar = _webinarRepository.FindById(row.idWebinar);

                //if order's placed before event - override starting point
                if (webinar.Date > orderDate)
                    expryDate = webinar.Date.AddMonths(6);
                if (regType.ShowRecordingNotifications.Equals("yes", StringComparison.OrdinalIgnoreCase))
                {
                    return expryDate;
                }
                // now we only addressing Live+5
                //update to pull LivePlusFive value from database
                var forceToMidnight = Convert.ToDateTime(webinar.LivePlusFiveValue.ToShortDateString()).AddHours(23).AddMinutes(59);
                return forceToMidnight;
            }
            catch (Exception exception)
            {
                _logger.FatalException("CalculatePostEventMaterialsAccessExpiry", exception);
                throw;
            }


        }

        public void GetJoinUrl(OrderRow row)
        {
            Order order = row.Order;

            if (row.Webinar.Status != WebinarStatus.Active && row.Webinar.Status != WebinarStatus.InProgress || row.Webinar.CitrixJoinInfoAvailable())
            {
                //if not initialized, don't hit Citrix
                return;
            }
            if (row.CitrixJoinUrl == null && row.RegistrationType.ShowLiveNotifications == "Yes"
                && (row.Webinar.Status == WebinarStatus.Active || row.Webinar.Status == WebinarStatus.InProgress))
            {
                var regKeyResponse = CreateRegistrantKey(order.FirstName, order.LastName
                    , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);

                if (ReferenceEquals(null, regKeyResponse))
                {
                    _logger.ErrorException(
                        "CreateRegistrantKey failed on Webinar: " + row.Webinar.idWebinar + " email: " +
                        order.BillingEmail,
                        new NullReferenceException("Attempt to CreateRegistrantKey failed on Webinar: {0} email: {1}" +
                                                   row.Webinar.idWebinar + " email: " + order.BillingEmail));
                }
            }
        }

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {
            var thisDiscount = GetDiscountByCode(code);
            var hasRemaining = CalculateCreditsRemain(thisDiscount);

            if (!ReferenceEquals(null, thisDiscount))
            {
                if (hasRemaining >= row.RegistrationType.CreditCost)
                {
                    thisDiscount = RedeemDiscount(thisDiscount, row);
                    thisDiscount.Notes =
                    "Order is fully discounted. " + (hasRemaining - row.RegistrationType.CreditCost).ToString().Replace(".00", "") + " will remain.";

                    row.Discount = thisDiscount;



                    _logger.Info("ApplyDiscountCode: " + row.Discount.DiscountCode + " idOrder: " + row.idOrder);
                }
                else
                {
                    thisDiscount.Notes =
                    "Insufficient credits: " + hasRemaining.ToString().Replace(".00", "") + " remain but " + row.RegistrationType.CreditCost.ToString().Replace(".00", "") + " are required.";
                    _logger.Info("ApplyDiscountCode failed due to insufficient credits: " + thisDiscount.DiscountCode + " idOrder: " + row.idOrder + +hasRemaining + " remain but " + row.RegistrationType.CreditCost + " are required.");
                }

                return thisDiscount;
            }
            else
            {
                return null;
            }

        }

        public Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink, OrderGenesis orderGenesis = OrderGenesis.ImportedForExistingUser)
        {
            decimal optionsPrice = GetAdditionalLocationsPricing(currentOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar);

            //ProcessDiscountCodes(currentOrder);
            CalculateOrderCost(currentOrder, optionsPrice);

            try
            {
                _logger.Info("SaveOrderChanges: {0}", currentOrder.idOrder);

                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, 0);

                // If linkToVerifyAccount is true, then we know that the user was created during an importation. When that occurs, 
                // we don't want to send the normal register user email. We want to roll those details into this confirmation
                // notification (OrderSubmitted notification). 

                //So, if we have the confirmChangeEmailLink, we can include it in 
                // the notification confirming registration for this webinar.  
                bool linkToVerifyAccount = !string.IsNullOrWhiteSpace(confirmChangeEmailLink);


                //this is just a smoke test, right? currentOrder.Webuser should never be null at this point. 
                _logger.Info("currentOrder.WebUser ({1}) is{0}null", currentOrder.WebUser == null ? " " : " not ", currentOrder.WebUser.email);

                var orderSubmittedViewModel = new ConfirmOrderMessage
                {
                    ConfirmChangeEmailUrl =
                        linkToVerifyAccount
                            ? string.Concat(confirmChangeEmailLink.Replace(DomainConstants.Blank, string.Empty),
                                currentOrder.WebUser.LastName.ToLower())
                            : string.Empty,
                    idOrder = updatedOrder.idOrder,
                    Order = updatedOrder,
                    OrderGenesis = orderGenesis,
                    UserCreatedOnImport = linkToVerifyAccount
                };


                //  The following "if" statement suppresses the OrderSubmitted notification where anonymous user has
                //  clicked the SignUp button or the order was migrated. The notification is still sent, it is just that
                //  it will be sent when the user clicks the "Bill Me" button on the 3rd tab of the cart. Not now.
                if (!currentOrder.Origin.Equals("Migrator", StringComparison.OrdinalIgnoreCase) &&
                    !currentOrder.Origin.Equals(DomainConstants.Cart, StringComparison.OrdinalIgnoreCase) &&
                    !currentOrder.Origin.Equals(DomainConstants.OriginImportedACS, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.Info("Adding Event for Order {0}", currentOrder.idOrder);

                    AddEvent(new OrderSubmittedEvent<ConfirmOrderMessage>
                    {
                        EventObject = orderSubmittedViewModel,
                        RelativePath = string.Empty
                    });

                    //it appears that the only way an event could be added within this method is a true response in the above If test.
                    // hence this statement can be safely moved up here? [dar] not sure what you mean.
                    foreach (var evt in GetEvents())
                    {
                        _logger.Info("OrderSubmittedEvent being raised for order {0}", orderSubmittedViewModel.idOrder);
                        _ttsConfig.NotificationEventBus.RaiseEvent(evt);
                    }

                }

                Clear();

                return updatedOrder;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("SaveOrderChanges method: {0}", exception.Message), exception);
            }
            //not seeing anyway the 'return null' could be hit but resharper is not flagging it as unreachable.
            _logger.Error(string.Format("SaveOrderChanges method fell all the way thru processing {0}", currentOrder.BillingEmail));
            return null;
        }

        public virtual IList<Order> SelectOrdersWithArchivedWebinars(int idUser)
        {

            return _orderRepository.SelectOrdersWithArchivedWebinars(idUser);
        }

        public IList<Order> SelectOrdersWithRecordedWebinars(int idUser)
        {
            return _orderRepository.SelectOrdersWithRecordedWebinars(idUser);
        }

        public IList<Order> SelectOrdersWithScheduledWebinars(int idUser)
        {
            return _orderRepository.SelectOrdersWithScheduledWebinars(idUser);
        }

        public int CheckUserForRecordingAccess(int w, int u)
        {
            return _orderRepository.AccessToPostEventMaterials(w, u);
        }

        private Discount RedeemDiscount(Discount discount, OrderRow row)
        {
            var forNotes = new StringBuilder();
            //
            _logger.Info("Discount: RedeemDiscountStarts: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}", discount.DiscountCode, discount.DateValidFrom, discount.DateValidTo, CalculateCreditsUsed(discount), CalculateCreditsRemain(discount));
            decimal creditsRemain = CalculateCreditsRemain(discount);

            switch (discount.DiscountType)
            {
                case DiscountType.Compensation:
                case DiscountType.ComplianceSeries:
                case DiscountType.DirectorSeries:
                case DiscountType.FivePart:
                case DiscountType.FourPart:
                case DiscountType.ThreePart:
                case DiscountType.Package:
                case DiscountType.Promo:
                case DiscountType.Subscription:
                    if (creditsRemain > 0)
                    {
                        forNotes.Append(CalculateDiscountRedemption(discount, row));
                    }
                    else
                    {
                        forNotes.Append("No Credits Remain. Credits used = " + creditsRemain);
                    }
                    _logger.Info(forNotes.ToString());
                    _discountRepository.SaveChanges(discount);
                    break;
            }
            return discount;
        }

        public Discount CalculateDiscountRedemption(Discount discount, OrderRow row)
        {
            if (!discount.Status.ToLower().StartsWith("a"))
            {
                _logger.Warn("Discount redemption attempted on: " + discount.idDiscount + " - " + row.idOrder);
                discount.Notes = ("This Discount Code " + discount.DiscountCode + " is expired.<br> For more info contact us by using the <i>Help & Feedback</i> button below<br> or emailing <b>Support@ttsTrain.com</b>.");
            }
            var forNotes = new StringBuilder();
            var existingDiscount = discount;
            var regTypeLabel = GetRegTypeOfOrderRow(row.idRegType).OptionLabel;
            decimal creditsRemain = CalculateCreditsRemain(discount);
            decimal creditsUsed = CalculateCreditsUsed(discount);


            decimal thisUseCost = GetRegTypeOfOrderRow(row.idRegType).CreditCost;

            if (discount.DiscountType == DiscountType.Subscription)
            {
                var remainsAfterThisUse = creditsRemain - thisUseCost;

                if (discount.DateValidTo > discount.DateValidFrom)
                {
                    forNotes.AppendFormat(" Your subscription will expire on " +
                                          discount.DateValidTo.ToShortDateString() + ".");
                }
                else if (discount.DateValidTo < discount.DateValidFrom)
                {
                    forNotes.AppendFormat(" Your subscription expired on " +
                                          discount.DateValidTo.ToShortDateString());
                }
                else if (creditsRemain > 0)
                {
                    if (creditsRemain >= thisUseCost)
                    {

                        if (remainsAfterThisUse >= thisUseCost)
                        {

                            forNotes.AppendFormat(
                                " If applied to this order, {0} {1} will be deducted <br>from your package with {2} remaining.",
                                thisUseCost.ToString().Replace(".00", ""),
                                thisUseCost > 1 ? "credits" : "credit",
                                    remainsAfterThisUse.ToString().Replace(".00", ""));

                        }
                        else
                        {
                            if (remainsAfterThisUse == 0)
                            {
                                forNotes.AppendFormat(
                                    " If applied to this order, {0} credit will be deducted <br>from your package with none remaining.",
                                    thisUseCost.ToString().Replace(".00", ""));
                            }
                            else
                            {
                                forNotes.AppendFormat(
                                   " If applied to this order, {0} credit will be deducted <br>from your package with {1} remaining.",
                                   thisUseCost.ToString().Replace(".00", ""),
                                   remainsAfterThisUse.ToString().Replace(".00", ""));
                            }
                        }
                    }
                }
                else
                {

                    forNotes.AppendFormat(" Insufficient credits remaining. Required: {0} Available: {1}", thisUseCost.ToString().Replace(".00", ""), creditsRemain.ToString().Replace(".00", ""));
                }
            }
            if (discount.DiscountType == DiscountType.Compensation)
            {
                forNotes.AppendFormat("Applying discount code: {0}", discount.DiscountCode);
            }
            if (discount.DiscountType == DiscountType.Promo)
            {
                forNotes.AppendFormat("Applying discount code: {0}", discount.DiscountCode);
            }

            discount.Notes = forNotes.ToString();

            return discount;
        }

        public decimal CalculateCreditsRemain(Discount userDiscount)
        {
            // this and the CalculateCreditUsed method
            // are copy/paste replicates of the OrderReposistory versions
            if (userDiscount.DiscountType == DiscountType.Subscription &&
                userDiscount.DateValidTo > userDiscount.DateValidFrom) return 100; // date-based subscription, # doesn't really matter
            if (userDiscount.DiscountType == DiscountType.ComplianceSeries) return 100; // date-based subscription, # doesn't really matter

            var ordersWithDiscount = GetOrdersByDiscount(userDiscount.idDiscount)
                .Where(o => o.OrderDate > userDiscount.DateVerified
                            || (o.InvoiceDetail.Contains("DiscountIsApplied"))
                            && (o.OrderStatus == OrderStatus.Submitted || o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid)
                            && !o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title.StartsWith("Compliance Perspe"));

            var creditsUsed = 0M;

            if (ordersWithDiscount.Any())
                foreach (var order in ordersWithDiscount)
                {
                    creditsUsed +=
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.CreditCost;
                }
            return userDiscount.TotalCount - creditsUsed;
        }

        public decimal CalculateCreditsUsed(Discount userDiscount)
        {

            // this and the CalculateCreditRemain method
            // are copy/paste replicates of the OrderRepository versions
            var ordersWithDiscount = GetOrdersByDiscount(userDiscount.idDiscount)
                  .Where(o => o.OrderDate > userDiscount.DateVerified);
            var credits = 0M;
            if (ordersWithDiscount.Any())
                foreach (var order in ordersWithDiscount)
                {
                    credits +=
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.CreditCost;
                }

            return credits;
        }

        public void CreateTestRegistration(Webinar webinar)
        {
            var idRegType = GetAllPossibleOptionsByWebinarId(webinar.idWebinar, false);

            var live_id = 0;

            foreach (var _regType in idRegType)
            {
                if (_regType.Key.OptionLabel.Contains("Five"))
                    live_id = _regType.Key.idRegType;
                ;
            }

            var row = CreateOrderRow(webinar, null, live_id);
            var orders = new List<Order>();
            var order = CreateNewOrder(_affiliateRepository.FindById(19), GetWebUser(1), webinar, row, "testing");

            AssignWebUserToOrder(GetWebUser(1), order);
            AssignAffiliateToOrder(19, order);

            order.OrderStatus = OrderStatus.Submitted;

            orders.Add(order);

            FireSendConnectionInfoNotificationEvent(orders, false); // this is where a single email is specified

            DeleteOrder(order.idOrder);
        }

        public int CheckIfEmailAlreadyRegisteredForWebinar(int idWebinar, string email)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.CheckIfEmailAlreadyRegisteredForWebinar(email, idWebinar);
        }

        public bool UserHasMultipleEvents(int idUser)
        {
            var orders = GetOrdersByUserId(idUser).Where(o => o.OrderStatus == OrderStatus.InProcess);
            return orders.Count() > 1;
        }

        public Order GetOrderByJoinCode(string joinCode)
        {
            return _orderRepository.GetOrderByJoinCode(joinCode);
        }

        public List<Registrant> GetCitrixRegistrantsByWebinar(int webinarId)
        {
            return _orderRepository.GetCitrixRegistrantsByWebinar(_webinarRepository.FindById((webinarId)));
        }


        public IList<WebUser> GetWebUsersOfDiscount(int idDiscount)
        {
            return _webUserRepository.GetWebUsersOfDiscount(idDiscount);
        }

        public string CheckOrderComments()
        {
            int totalNumberOrders = 0;
            List<Order> orders = _orderRepository.GetOrdersAll(19, out totalNumberOrders).ToList();

            foreach (var order in orders)
            {
                //if (order.AdminComments != null && !order.AdminComments.StartsWith("["))
                //{
                //    var AdminResult = JsonHelpers.IsValidObjectSingle(order.AdminComments);
                //    if (AdminResult != "isSingle")
                //    {
                //        _logger.Warn("invalid Json on Admin: " + order.idOrder + " | " + order.AdminComments);
                //    }
                //}
                if (!string.IsNullOrEmpty(order.InvoiceDetail))
                {
                    var InvoiceDetail = JsonHelpers.IsValidObjectSingle(order.InvoiceDetail);
                    if (InvoiceDetail != "isSingle")
                    {
                        _logger.Warn("invalidJson_InvoiceDetail: {" + order.idOrder + "}," + order.InvoiceDetail);
                    }
                }
                //if (order.AffiliateComments != null && !order.AffiliateComments.StartsWith("["))
                //{
                //    var AffResult = JsonHelpers.IsValidObjectSingle(order.AffiliateComments);
                //    if (AffResult != "isSingle")
                //    {
                //        _logger.Warn("invalid Json on Aff: " + order.idOrder + " | " + order.AffiliateComments);
                //    }
                //}
                //if (order.UserComments != null && !order.UserComments.StartsWith("["))
                //{
                //    var UserResult = JsonHelpers.IsValidObjectSingle(order.UserComments);
                //    if (UserResult != "isSingle")
                //    {
                //        _logger.Warn("invalid Json on User: " + order.idOrder + " | " + order.UserComments);
                //    }
                //}
            }

            return null;
        }

        //private void RejectDiscount(OrderRow orderRow)
        //{
        //    orderRow.Discount.CreditsRemain++;
        //    //according to legacy code but can a condition exist 
        //    //  a non-valid discount resulted in a decrement.
        //}

        public Discount GetDiscountById(int id)
        {
            var discount = _orderRepository.FindDiscountById(id);
            return discount;
        }

        public IList<Order> GetV3OrdersByWebinar(int idWebinar)
        {
            return _webinarRepository.GetOrdersByWebinar(idWebinar).ToList();
        }


        public WebUser GetWebUserWithAddressAndInstitution(int idUser)
        {
            return _webUserRepository.GetWebUserByIdLoadedWithAddressesAndInstitution(idUser);
        }


        public Registrant CreateRegistrantKey(string firstName, string lastName, string billingEmail, int webinarId,
            string webinarKey)
        {
            var webinar = _webinarRepository.FindById(webinarId);
            var orgKey = _ttsConfig.ConvertToCitrixOrgKey(webinar.OrganizerKey);
            var cWebinarKey = _ttsConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey);

            try
            {
                var api = new RegistrantsApi();
                var apiResponse = api.createRegistrant(webinar.OrganizerOAuthKey, orgKey, cWebinarKey, "application/vnd.citrix.g2wapi-v1.1+json", false, new RegistrantFields { firstName = firstName, lastName = lastName, email = billingEmail }); // {};

                Registrant reg = new Registrant
                {
                    firstName = firstName,
                    lastName = lastName,
                    email = billingEmail,
                    joinUrl = apiResponse.joinUrl,
                    registrantKey = apiResponse.registrantKey
                };
                _logger.Info("CreateRegistrantKey returns: " + billingEmail + ", " + webinarKey + " - Response = " + apiResponse);

                return reg;
            }
            catch (WebException webException)
            {
                _logger.ErrorException(string.Format("WebException CreateRegistrantKey: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, webException.Message), webException);
                var httpWebResponse = webException.Response as HttpWebResponse;

                if (!ReferenceEquals(httpWebResponse, null))
                {
                    string responsePayload = ProcessErrorByStatusCode(httpWebResponse);

                    _logger.Error("Citrix Message: {0}", responsePayload);

                }
                return new Registrant { firstName = webException.Message };
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Exception CreateRegistrantKey: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, exception.Message), exception);
                return new Registrant { firstName = exception.Message };
            }

        }

        /// <summary>
        /// Error messages for codes taken from https://developer.citrixonline.com/citrix-api-http-status-codes
        /// </summary>
        private static string ProcessErrorByStatusCode(HttpWebResponse httpWebResponse)
        {
            string responsePayload = string.Empty;

            switch (httpWebResponse.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    {
                        responsePayload =
                            @"The request is invalid. This can be caused by an incorrect argument such as 'time' instead of'times' or may be caused by trying to book sessions that overlap in time, dates beyond one year, and if the calls fail to find the requested data.";
                        break;
                    }
                case HttpStatusCode.Unauthorized:
                    {
                        responsePayload =
                            @"This response alerts you that there is a problem with your account. The account_token could be incomplete or incorrect, your account could be expired, or the authorization token (oauth_token) may be invalid.";
                        break;
                    }
                case HttpStatusCode.Forbidden:
                    {
                        if (httpWebResponse.StatusDescription.Trim().Equals("Forbidden", StringComparison.OrdinalIgnoreCase))
                        {
                            responsePayload =
                                "This response is received when internal rules or limits have been reached. For example, in the PUT or CREATE calls, this will be returned if an incorrect data key is used, the collaboration session is full, or if the event has ended.";
                        }
                        else if (httpWebResponse.StatusDescription.Trim().Equals("Access Denied", StringComparison.OrdinalIgnoreCase))
                        {
                            responsePayload =
                                "The user making this call is not authorized to make the call. A user with appropriate admin roles must make the call.";
                        }
                        break;
                    }
                case HttpStatusCode.NotFound:
                    {
                        responsePayload =
                            "Occurs when an incorrect key value, a non-existent or deleted data key, or a key for an event in the past is passed.";
                        break;
                    }
                case HttpStatusCode.MethodNotAllowed:
                    {
                        responsePayload =
                            "Can occur if your calls pass through an ISP that does not allow POST methods. If this error arises, contact your ISP.";
                        break;
                    }
                case HttpStatusCode.Conflict:
                    {
                        responsePayload = "This occurs if you attempt to create a duplicate of a unique resource.";
                        break;
                    }
                case HttpStatusCode.InternalServerError:
                    {
                        responsePayload =
                            "There is an internal server error. This could be caused by a failed or partial connection, or other transitory conditions. The request may be retried, but it may have the same result.";
                        break;
                    }
                case HttpStatusCode.BadGateway:
                    {
                        if (httpWebResponse.StatusDescription.Trim().Equals("Bad Gateway"))
                        {
                            responsePayload =
                                "The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request. Simplify the data request and ensure post requests are within available periods.";
                        }
                        else if (httpWebResponse.StatusDescription.Trim().Equals("Illegal Account"))
                        {
                            responsePayload =
                                "Accounts without billing IDs for which tollFreeProvisioned is true. This is a hard error on the account. Contact developer-support@citrixonline.com to clear the error and reset your account.";
                        }
                        break;
                    }
                case HttpStatusCode.GatewayTimeout:
                    {
                        responsePayload =
                            "The server, while acting as a gateway or proxy, did not receive a timely response from the upstream server.";
                        break;
                    }

                default:
                    {
                        if (httpWebResponse.StatusDescription.Trim().Equals("Unprocessable Entity")) // 422
                        {
                            responsePayload =
                                "The request was formed correctly but was unable to be followed due to semantic errors.";
                        }
                        else if (httpWebResponse.StatusDescription.Trim().Equals("Too Many Requests")) // 429
                        {
                            responsePayload =
                                "The broker returns this code if, for a given user account, too many requests are received in a specific time frame. Calls can be tried again later.";
                        }
                    }
                    break;
            }
            return responsePayload;
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

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _affiliateRepository.Dispose();
                _orderRepository.Dispose();
                _regTypeRepository.Dispose();
                _webinarRepository.Dispose();
                _webUserRepository.Dispose();

            }
            _disposed = true;
        }
    }
}
