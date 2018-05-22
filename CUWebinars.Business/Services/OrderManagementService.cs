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
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BrockAllen.MembershipReboot;
using LogMeIn.GoToWebinar.Api;
using LogMeIn.GoToWebinar.Api.Model;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Notification;
using CUWebinars.Web.Models;
using Mandrill.Model;
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


        public Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow,
            string origin = null)
        {
            try
            {
                var existingOrder = _orderRepository.FindOrderForUserByWebinarId(webUser.idUser, webinar.idWebinar);
                if (existingOrder != null && existingOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar != 2520)
                {
                    _logger.Warn("CreateNewOrder found and returned existing: " + existingOrder.idOrder);
                    return existingOrder;
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateNewOrder: ", ex);
            }

            var order = _orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow, origin);
            return order;
        }

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation,
            int registrationType)
        {
            try
            {
                var regType = _regTypeRepository.FindRegType(registrationType);

                //if (additionalLocation != null)
                //{
                //    foreach (var addLoc in additionalLocation)
                //    {
                //        addLoc.Price = GetCostOfAdditionalLocations(additionalLocation, webinar.idWebinar).Item2;
                //    }
                //}

                OrderRow row = _orderRepository.CreateOrderRow(webinar, additionalLocation, regType);

                return row;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    "overload of CreateOrderRow method webinar:  " + webinar.idWebinar + " regType: " + registrationType,
                    exception);

                throw;
            }
        }

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation,
            RegType registrationType)
        {
            try
            {
                _orderRepository.CreateOrderRow(webinar, additionalLocation, registrationType);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    "CreateOrderRow method webinar:  " + webinar.idWebinar + " regType: " + registrationType.idRegType,
                    exception);

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
                _logger.ErrorException(
                    string.Format("CreateAdditionalLocation method - values passed in {0},{1},{2}.", email, price,
                        fullname), exception);
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
        public IDictionary<RegType, bool> GetOptionsAvailableToExistingOrder(int id, bool b, Order order)
        {

            string cachKey = "options-" + id;
            //var options = _cachingService.Get(cachKey);

            //if (options == null)
            //{
            var options = _regTypeRepository.FindRegTypesAvailableToExistingOrder(id, false, order);

            //    // keeps options object in cache for 1 hour.
            //    _cachingService.Add(cachKey, options, DomainConstants.BuildUtcNowAsCts.AddHours(1));
            //}

            return (IDictionary<RegType, bool>)options;
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

        public IDictionary<RegType, bool> GetAllPossibleRegTypesByWebinarId(int idWebinar, bool detached)
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


        public IList<RegType> GetAllPossibleRegTypesByWebinarId(int idWebinar)
        {
            IList<RegType> regTypes = GetAllPossibleRegTypesByWebinarId(idWebinar, false).Select(r => r.Key).ToList();
            return regTypes;
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
            OrderRow row = _orderRepository.GetOrderRowById(idOrderRow);

            if (row == null)
            {
                row = _orderRepository.GetOrderById(idOrderRow).OrderRows
                    .SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            }
            return row;
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

        //public object SearchRegistrations(int affiliateID, IList<int> excludeUserIDs, int skip, int take, string search)
        //{
        //    return
        //        _orderRepository.SearchOrders(affiliateID, excludeUserIDs, skip, take, search);
        //}

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

        public Affiliate DetermineAffiliateByAlternativeMeans(int idUser, int sessionAff)
        {
            var buildDABAMStory = new StringBuilder();
            buildDABAMStory.Append("Starting with " + idUser);
            string cachKey = "webUserId-" + idUser;
            //var affiliateIds = _cachingService.Get(cachKey) as IList<int>;
            IList<int> affiliateIds = null;

            if (affiliateIds == null)
            {
                affiliateIds = _orderRepository.FindOrdersByUserId(idUser)
                    .Where(o => o.idAffiliate != 19)
                    .OrderByDescending(o => o.OrderDate)
                    .Distinct()
                    .Select(o => o.idAffiliate)
                    .ToList();
            }

            if (affiliateIds.Any())
            {
                try
                {
                    buildDABAMStory.Append(" found " + string.Join(", ", affiliateIds));
                    //  get the most recent
                    int affiliateIdForOrder, mostRecentAffiliateId;
                    affiliateIdForOrder = affiliateIds.First();

                    // The history is of more than 1 affiliate
                    if (affiliateIds.Count() > 1)
                    {
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
                            var ordersByThisAff = _orderRepository.FindOrdersByUserId(idUser)
                                .Where(o => o.idAffiliate == group.Key
                                            && (o.OrderStatus == OrderStatus.Paid ||
                                                o.OrderStatus == OrderStatus.Submitted ||
                                                o.OrderStatus == OrderStatus.Billed
                                                ))
                                .ToList().Count;

                            buildDABAMStory.Append(" Number of orders by " + _affiliateRepository.FindById(group.Key).ttsDomain + " was " + ordersByThisAff);

                            numberOfUsesOfMostRecentAffiliate = _orderRepository.FindOrdersByUserId(idUser)
                                .Where(o => o.idAffiliate == affiliateIds.FirstOrDefault()
                                            && (o.OrderStatus == OrderStatus.Paid ||
                                                o.OrderStatus == OrderStatus.Submitted ||
                                                o.OrderStatus == OrderStatus.Billed))
                                .ToList().Count;

                            if (ordersByThisAff > mostUses)
                            {
                                buildDABAMStory.Append(" ordersByThisAff (" + _affiliateRepository.LoadById(group.Key).ttsDomain + " had " + ordersByThisAff
                                    + ") is > than previous mostUses " + mostUses);

                                mostUses = ordersByThisAff;
                                mostUsedAffiliateId = group.Key;
                                affiliateIdForOrder = group.Key;
                            }


                            if (mostUses >= 2 * numberOfUsesOfMostRecentAffiliate)
                            {
                                //
                                affiliateIdForOrder = mostUsedAffiliateId;
                            }
                        }

                        if (mostUses >= 2 * numberOfUsesOfMostRecentAffiliate)
                        {

                            buildDABAMStory.Append(" based on most uses order is awarded to  "
                                + _affiliateRepository.LoadById(affiliateIdForOrder).ttsDomain + " based on mostUses:  >= 2 * numberOfUsesOfMostRecentAffiliate" + mostUses);

                            _logger.Info("DetermineAffiliateByAlternativeMeans by mostUses: " + idUser + " awarded: " +
                                         mostUsedAffiliateId);
                            affiliateIdForOrder = mostUsedAffiliateId;
                        }
                    }

                    var affiliate =
                        _affiliateRepository.LoadById(
                            affiliateIdForOrder); // _cachingService.Get(cachKey) as Affiliate;

                    if (affiliate == null)
                    {
                        affiliate =
                            _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder); // use the most recent
                        //affiliate = _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder, a => a.WebUser); // use the most recent
                        if (affiliate == null)
                        {
                            affiliate = GetAffiliateById(19);
                        }
                    }
                    _logger.Info("DetermineAffiliateByAlternativeMeans returned: " + affiliate.idUserAff + " for: " +
                                 idUser);
                    _logger.Info(buildDABAMStory.ToString());
                    return affiliate;
                }
                catch (Exception e)
                {
                    buildDABAMStory.Append("  ERROR!! " + e.Message);
                    _logger.Info(buildDABAMStory.ToString());
                    return GetAffiliateById(19);
                }
            }

            buildDABAMStory.Append(" Didn't find any besides 19!");
            _logger.Info(buildDABAMStory.ToString());

            return GetAffiliateById(19); ;
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

            Debug.Assert(row != null, "CalculateOrderCost OrderRow object should always have a value here.");
            if (row.RegistrationType != null)
            {
                row.UnitPrice = (decimal)row.RegistrationType.Price;
            }
            else
            {
                _logger.Warn("CalculateOrderCost did not find row when processing " + order.idOrder);
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                // todo: refactor to populate all regtype values as AppVars.
                var regTypePricing = dataOperations.GetCostOfRegtype(row.idRegType);
                row.UnitPrice = regTypePricing;

                row.RegistrationType = GetRegTypeOfOrderRow(row.idRegType);
                if (row.RegistrationType == null)
                {
                    _logger.FatalException("CalculateOrderCost could not determine regType!!",
                        new Exception("CalculateOrderCost could not determine regType", null));
                    throw new Exception("CalculateOrderCost could not determine regType");
                }

            }
            //Calculate row price before discount
            if (row.AdditionalLocation != null)
            {
                if (
                    order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active)
                        .Webinar.Title.Contains("Compliance Perspectives"))
                {
                    if (row.AdditionalLocation.Count > 3)
                    {
                        totalOptionsPrice = row.AdditionalLocation.Count - 3 * optionsCost;
                        // cost * number of additional locations
                    }
                }
                else
                {
                    totalOptionsPrice = row.AdditionalLocation.Count * optionsCost;
                    // cost * number of additional locations
                }
            }

            row.RowPrice = row.UnitPrice + totalOptionsPrice;
            pricesAndDiscounts.UnitPrice = row.UnitPrice;

            //Calculate discount. 
            decimal discountTotal = 0;

            if (row.Discount != null && row.Discount.PercentOff != 0.0M
                && !row.Webinar.Title.Contains("Webinar Subscription"))
            {
                var creditsRemain = CalculateCreditsRemain(row.Discount);

                if (row.Discount.DiscountType == DiscountType.Subscription)
                {
                    _logger.Info("CalculateOrderCost begin discount create " + row.Discount.idDiscount + " on " + row.idOrder +
                                 " found "
                                 + creditsRemain + " against " + row.RegistrationType.CreditCost);

                    if (creditsRemain >= row.RegistrationType.CreditCost)
                    {
                        discountTotal = row.RowPrice * row.Discount.PercentOff / 100;
                        if (row.Discount.DateValidFrom != row.Discount.DateValidTo && row.RegistrationType.ShowShippedNotifications.ToLower() == "yes")
                        {
                            discountTotal = discountTotal - 50;
                            var newJson = new JProperty(
                                "ShippingSurcharge",
                                new JObject(
                                    new JProperty("OriginalDiscountTotal",
                                        discountTotal + 50),
                                    new JProperty("UpdatedDiscountTotal",
                                        discountTotal)
                                ));

                            order.AdminComments = JsonHelpers.ReplaceJsonWithStoredField(order.AdminComments, newJson,
                                "ShippingSurcharge");
                        }
                        _logger.Info("COC create discount  " + row.Discount.idDiscount + " on " + row.idOrder +
                                     " found "
                                     + creditsRemain + " against " + row.RegistrationType.CreditCost);
                    }
                    else
                    {
                        if (creditsRemain >= 0)
                        {
                            discountTotal = 265 * creditsRemain;
                            _logger.Info("COC create partial discount  " + row.Discount.idDiscount + " on " +
                                         row.idOrder + " found "
                                         + creditsRemain + " against " + row.RegistrationType.CreditCost);
                        }
                        else
                        {
                            discountTotal = 0;
                            _logger.Info("COC failed to create discount  " + row.Discount.idDiscount + " on " +
                                         row.idOrder + " found "
                                         + creditsRemain + " against " + row.RegistrationType.CreditCost);
                            row.Discount = null;
                        }
                    }
                }
                else
                {
                    discountTotal = row.RowPrice * row.Discount.PercentOff / 100;
                    _logger.Info("COC found non WPS discount  " + row.Discount.idDiscount + " on " + row.idOrder +
                                 " found "
                                 + creditsRemain + " against " + row.RegistrationType.CreditCost);
                }
            }
            else if (row.Discount != null && row.Discount.FlatOff != 0.0M)
            {
                discountTotal = row.Discount.FlatOff;
                _logger.Info("COC found non flatOff discount " + row.Discount.idDiscount + " on " + row.idOrder +
                             " found " + row.Discount.FlatOff);
            }

            if (discountTotal >= row.RowPrice)
            {
                discountTotal = row.RowPrice;
            }

            if (row.Discount != null && row.Discount.DiscountCode.ToLower().Contains("expired"))
            {
                row.Discount = null;
            }
            row.RowPrice -= discountTotal;
            pricesAndDiscounts.TotalDiscount = discountTotal;
            pricesAndDiscounts.TotalCostOfOptions = totalOptionsPrice;

            pricesAndDiscounts.TaxAmount = 0;

            if (order.BillingState == "WI"
                && !row.RegistrationType.OptionLabel.StartsWith("Live Plus Five")
                && !row.RegistrationType.SKU.Contains("WSP")
                && row.RowPrice > 0
            )
            {
                row.Tax = Math.Round(row.RowPrice * Convert.ToDecimal(.055), 2);
                pricesAndDiscounts.TaxAmount = Math.Round(row.RowPrice * Convert.ToDecimal(.055), 2);
                _logger.Info("COC found tax on: " + row.idOrder + " found " + pricesAndDiscounts.TaxAmount);
            }

            //Calculate order total
            order.Total = row.RowPrice + pricesAndDiscounts.TaxAmount;
            pricesAndDiscounts.Discount = row.Discount;
            pricesAndDiscounts.TotalOrderPrice = order.Total - order.TotalPaid;
            pricesAndDiscounts.TotalPaid = order.TotalPaid;

            return pricesAndDiscounts;
        }

        public decimal DiscountCreditUnitCost { get; private set; }

        //public void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false,
        //    Uri url = null)
        // depricated by Mandrill sender
        //}

        public void FireAdminEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients,
            bool resending = false)
        {
            AddEvent(new AdminEmailSendShippedOrderEvent<Order>
            {
                EventObject = order,
                Recipients = recipients,
                ResendEvent = resending
            });


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

            foreach (
                var evt in
                GetEvents().OfType<OrderSubmittedAdditionalLocationEvent<AdditionalLocationOrderDetailsMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            //int rowsUpdated = _orderRepository.SaveChanges();
        }

        public void FireMandrillNotificationEvent(string toEmail, string subject, string body)
        {
            //TODO: refactor recordingIsPostedV2Message to a more generic name
            var recordingIsPostedV2Message = new MandrillNotificationMessage()
            {
                Recipients = toEmail.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                Subject = subject,
                Body = body,
            };

            AddEvent(new MandrillNotificationEvent<MandrillNotificationMessage> { EventObject = recordingIsPostedV2Message });

            foreach (var evt in GetEvents().OfType<MandrillNotificationEvent<MandrillNotificationMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();

            //
        }

        //public void FireSendRecordingIsPostedEvent(IList<Order> orders)
        //{

        //depricated by Mandrill sender
        //}


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

            //var idWebinar = orders[0].OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.idWebinar;

            //var citrixRegs = GetCitrixRegistrantsByWebinar(idWebinar);


            foreach (var order in orders)
            {
                //GenerateRegistrantKey(order);
                AddEvent(new SendConnectionInfoEvent<Order>
                {
                    EventObject = order,
                    ResendEvent = resending,
                    Details = order.NotificationStorage
                });
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

        //public void FireSendRecordingPostedNotificationEvent(IList<Order> orders)
        //{
        //    var title = orders[0].OrderRows.SingleOrDefault().Webinar.Title;

        //    _logger.Info("Begins RecordingPostedNotification for {1} with {0} orders.", orders.Count, title);
        //    var x = 0;
        //    foreach (var order in orders)
        //    {
        //        _logger.Info("RecordingPostedNotification {0} of {1} sent to {2} for {3}.", x, orders.Count,
        //            order.BillingEmail, title);

        //        var postEventPublishModel = new PostEventPublishModel()
        //        {
        //            Order = order
        //        };
        //        AddEvent(new SendRecordingPostedEvent<PostEventPublishModel>
        //        {
        //            EventObject = postEventPublishModel
        //        });
        //    }

        //    foreach (var evt in GetEvents().OfType<SendRecordingPostedEvent<PostEventPublishModel>>())
        //    {
        //        _ttsConfig.NotificationEventBus.RaiseEvent(evt);
        //    }

        //    Clear();

        //    int numRows = _orderRepository.SaveChanges();
        //}

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

        }
        public void FireOrderSubmitted2Event(string toEmail, string subject, string body)
        {
            var orderSubmitted2Message = new OrderSubmitted2Message()
            {
                Recipients = toEmail.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                Subject = subject,
                Body = body,
            };

            AddEvent(new OrderSubmitted2Event<OrderSubmitted2Message> { EventObject = orderSubmitted2Message });

            foreach (var evt in GetEvents().OfType<OrderSubmitted2Event<OrderSubmitted2Message>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }


        public string UpdateOrderChanges(Order newOrder, ref PricesAndDiscounts pricesAndDiscounts)
        {
            try
            {

                var originalOrder = GetOrderById(newOrder.idOrder);
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                var additionalLocationsPricing =
                    newOrder.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                        .Webinar.AdditionalLocationPrice;
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

                        .Replace(" Recording Only", "")
                        .Replace(" Plus Five", "+5");
                decimal pTotal = Convert.ToDecimal(preSaveValues.Split(',')[6]);
                int idRegTypeOfOrg = Convert.ToInt32(preSaveValues.Split(',')[5]);

                pricesAndDiscounts = CalculateOrderCost(newOrder, additionalLocationsPricing);

                if (originalOrder.OrderStatus == OrderStatus.Paid && pTotal != newOrder.Total)
                {
                    if (originalOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Discount == null)
                    {
                        newOrder.OrderStatus = OrderStatus.OutstandingBalance;
                    }
                    if (originalOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Discount != null && originalOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Discount.DiscountType != DiscountType.Subscription)
                    {
                        newOrder.OrderStatus = OrderStatus.OutstandingBalance;
                    }
                }

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
                            if (!string.IsNullOrEmpty(newOrder.InvoiceDetail) &&
                                newOrder.InvoiceDetail.Contains("OrderIsInvoiced"))
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
                                              row.RowPrice.ToString("C").Replace(".00", "") + ") on " +
                                              TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");

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
                                            new JProperty("OriginalInvoice",
                                                orgInvoiceDetails.First()["InvoiceId"].ToString()),
                                            new JProperty("OriginalDateOfInvoice",
                                                orgInvoiceDetails.First()["DateOfInvoice"].ToString()),
                                            new JProperty("OriginalTotal",
                                                orgInvoiceDetails.First()["AmountOfOrder"].ToString()),
                                            new JProperty("OriginalPercentPaid",
                                                orgInvoiceDetails.First()["PercentPaid"].ToString()),
                                            new JProperty("OriginalRoyaltyPaid",
                                                orgInvoiceDetails.First()["AmountOfRoyalty"].ToString()),
                                            new JProperty("OriginalAffiliate",
                                                orgInvoiceDetails.First()["Affiliate"].ToString()),
                                            new JProperty(adjustmentDirection, adustmentAmount),
                                            new JProperty("DateOfChange",
                                                TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
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
                                              row.RowPrice.ToString("C").Replace(".00", "") + ") on " +
                                              TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat) + ". ");


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
                                            new JProperty("OriginalInvoice",
                                                orgInvoiceDetails.First()["OriginalInvoice"].ToString()),
                                            new JProperty("OriginalDateOfInvoice",
                                                orgInvoiceDetails.First()["OriginalDateOfInvoice"].ToString()),
                                            new JProperty("OriginalTotal",
                                                orgInvoiceDetails.First()["OriginalTotal"].ToString()),
                                            new JProperty("OriginalPercentPaid",
                                                orgInvoiceDetails.First()["OriginalPercentPaid"].ToString()),
                                            new JProperty("OriginalRoyaltyPaid",
                                                orgInvoiceDetails.First()["OriginalRoyaltyPaid"].ToString()),
                                            new JProperty("OriginalAffiliate",
                                                orgInvoiceDetails.First()["OriginalAffiliate"].ToString()),
                                            new JProperty(adjustmentDirection, adustmentAmount),
                                            new JProperty("DateOfChange",
                                                TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeShortFormat)),
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
                _logger.ErrorException(
                    string.Format("SaveOrderChanges method: {0} - {1}", newOrder.idOrder, exception.Message), exception);
            }
            return "failed";
        }

        public void UpdateOrderWithUserEmail(int orderId, string email)
        {
            try
            {
                var order = _orderRepository.FindById(orderId);
                order.BillingEmail = email;
                // ADDED 1/17 to provide complete user details to order

                var user = _webUserRepository.GetWebUserByEmail(email);
                if (!ReferenceEquals(user, null))
                    _orderRepository.AssignWebUserToOrder(user, order);
                _orderRepository.SaveOrderChanges(order, null);

                RemoveDupedOrders(order);


            }
            catch (Exception ex)
            {
                _logger.FatalException("UpdateOrderWithUserEmail: ", ex);
                throw;
            }
        }

        public void UpdateOrderWithUserId(int orderId, int userId)
        {
            var user = _webUserRepository.FindByIdLoaded(userId);
            var order = _orderRepository.FindById(orderId);
            var oAffId = order.idAffiliate;
            order.AuditInfo = "{\"anon user becomes " + user.email + "\": " + order.AuditInfo + "}";

            if (orderId == 19)
                order.idAffiliate = DetermineAffiliateByAlternativeMeans(userId, oAffId).idUserAff;

            if (oAffId != order.idAffiliate)
                order.AuditInfo = "{\"anon user (to " + user.email + ") updates Affiliate from: " + oAffId + " to: " + order.idAffiliate + " " + order.AuditInfo + "}";

            order.idUser = userId;

            //RemoveDupedOrders(order);


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

        private void RemoveDupedOrders(Order order)
        {
            List<Order> orders =
                GetOrdersByEmail(order.BillingEmail, 19)
                    .Where(o => o.idOrder != order.idOrder)
                    .ToList();
            IList<int> iDsToRemove = new List<int>();

            var result =
                    orders
                        .SelectMany(o => o.OrderRows
                        .Where(r => r.RowStatus == OrderRowStatus.Active))
                        .OrderBy(o => o.Order.OrderDate)
                        .GroupBy(y => y.idWebinar)
                        .Where(g => g.Skip(1).Any())
                        .Select(g => g.Key)
                        .ToList()
                ;
            if (result.Any())
            {
                foreach (var i in result)
                {
                    var _orders =
                        orders.Where(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar == i)
                        .Skip(1).ToList();
                    foreach (var _orderId in _orders)
                    {
                        iDsToRemove.Add(_orderId.idOrder);
                    }
                }
            }

            foreach (var id in iDsToRemove)
            {
                var itemToRemove = orders.SingleOrDefault(o => o.idOrder == id);
                if (itemToRemove != null)
                {
                    itemToRemove.OrderStatus = OrderStatus.Canceled;
                    SaveOrderChanges(itemToRemove, null, null);
                    if (itemToRemove != null)
                        orders.Remove(itemToRemove);

                    _logger.Warn("Found and canceled duped order: " + id);
                }
            }
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
            //if ()
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
            var orderIds = _webinarRepository.GetV3OrdersIdsByWebinar(idWebinar);
            
            return _orderRepository.GetOrdersByIds(orderIds.ToArray()).ToList();
            //List<Order> orders = new List<Order>();
            //foreach (var orderId in orderIds)
            //{
            //    orders.Add(_orderRepository.GetOrderById(orderId));

            //}
            //foreach (var _o in orders)
            //{
            //    if (_o != null)
            //    {
            //        Debug.WriteLine("idOrder: " + _o.idOrder);

            //    }
            //    else
            //    {
            //        continue;
            //    }
            //}

            //return orders;

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

            try
            {
                //Debug.Assert(!string.IsNullOrWhiteSpace(row.CitrixJoinUrl), "CitrixJoinUrl should always be null or empty before this method is called as a pre-condition.");
                if (row.TtsJoinUrl != null && row.RegistrationType.ShowLiveNotifications.TrimEnd()
                        .Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    var registrant = CreateRegistrantKey(
                        order.FirstName ?? " ",
                        order.LastName ?? " ",
                        order.BillingEmail,
                        row.Webinar.idWebinar,
                        row.Webinar.WebinarKey
                    );

                    row.CitrixJoinUrl = registrant.joinUrl;
                    row.RegistrantKey = registrant.registrantKey.ToString();
                    var regKeyResponse = registrant.registrantKey.ToString();
                    if (string.IsNullOrWhiteSpace(regKeyResponse))
                    {
                        _logger.FatalException("GenerateRegistrantKey: ", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response for: " + order.idOrder));
                    }
                    else
                    {
                        _logger.Info("GenerateRegistrantKey for " + order.idOrder + " = " + regKeyResponse);
                    }

                    //if (row.AdditionalLocation != null)
                    //{
                    //    // This branch gets key for main Additional Locations
                    //    foreach (var addLoc in row.AdditionalLocation)
                    //    {
                    //        registrant = CreateRegistrantKey(
                    //            "c/o " + order.FirstName,
                    //            order.LastName ?? " ",
                    //            addLoc.Email,
                    //            row.Webinar.idWebinar,
                    //            row.Webinar.WebinarKey
                    //        );
                    //        addLoc.RegistrantKey = registrant.registrantKey.ToString();
                    //        addLoc.JoinURL = registrant.joinUrl;
                    //        regKeyResponse = registrant.registrantKey.ToString();
                    //        if (string.IsNullOrWhiteSpace(regKeyResponse))
                    //        {
                    //            _logger.FatalException("GenerateRegistrantKey creation (addLoc) failed.", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response for: " + order.idOrder));
                    //        }
                    //        else
                    //        {
                    //            _logger.Info("GenerateRegistrantKey (addLoc) for " + order.idOrder + " = " + regKeyResponse);
                    //        }
                    //    }
                    //}

                    SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.FatalException("GenerateRegistrantKey: ", ex);
            }

            SaveChanges();
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

        public string RemoveDiscountCode(string code, OrderRow row)
        {
            //Removes the discount from this order
            try
            {
                row.Discount = null;
                SaveOrderChanges(row.Order, null, null, orderGenesis: OrderGenesis.Resend);
                return "Suceeded";
            }
            catch (Exception ex)
            {
                _logger.FatalException("RemoveDiscountCode", ex);
                return "Failed";
            }

        }

        public Discount CreateWspCode(Order order)
        {
            _logger.Info("Create WSP: " + order.idOrder);
            var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            var whichTier = "";
            var totalCount = 0;

            switch (row.RegistrationType.OptionLabel)
            {
                case "Bronze":
                    whichTier = "BZ-";
                    totalCount = 5;
                    break;
                case "Gold":
                    whichTier = "GD-";
                    totalCount = 20;
                    break;
                case "Silver":
                    whichTier = "SL-";
                    totalCount = 10;
                    break;
                case "Platinum":
                    whichTier = "PL-";
                    totalCount = 50;
                    break;
                case "Diamond":
                    whichTier = "DM-";
                    totalCount = 100;
                    break;

                default:
                    whichTier = "unknownTier";
                    break;

            }
            if (whichTier == "unknownTier")
            {
                FireMandrillNotificationEvent(
                    "info@ttstrain.com",
                    "Failed to create WSP " + order.idOrder
                    , "Unknown tier: ");

                return null;
            }

            if (order.WebUser.idSubscriptionDiscount != null)
            {
                try
                {
                    var discount = GetDiscountById(order.WebUser.idSubscriptionDiscount.Value);
                    if (discount.idAffiliate != order.idAffiliate)
                    {
                        FireMandrillNotificationEvent(
                            "info@ttstrain.com",
                            "affiliate did not match existing WSP " + order.idOrder
                            , "Order Affiliate was: " + order.idAffiliate + " but existing WSP had: " + discount.idAffiliate);

                        var aff = GetAffiliateByIdLoaded(discount.idAffiliate);
                        order.Affiliate = aff;
                        order.idAffiliate = discount.idAffiliate;
                    }

                    if (discount.DiscountType == DiscountType.Subscription)
                    {
                        discount.DateValidTo = order.OrderDate;
                        discount.DateValidFrom = order.OrderDate;
                        discount.Cost = row.RowPrice;
                        discount.DateBilled = DateTime.UtcNow;
                        discount.DateVerified = DateTime.UtcNow;
                        discount.FlatOff = 0;

                        discount.PercentOff = 100;
                        discount.RenewalTerm = 0;
                        discount.Status = "Active";
                        discount.TotalCount += totalCount;
                        discount.idAffiliate = order.idAffiliate;
                        discount.Notes += " Renewed with " + totalCount + " on " + DateTime.Now.ToString("dd-MM-yyyy");

                    }

                    row.Discount = discount;
                    SaveOrderChanges(order, null, null);

                    FireMandrillNotificationEvent("2afda898.ttstrain.com@amer.teams.ms"
                        , "Existing WSP was renewed: " + order.idOrder
                        , "Renewal for : " + discount.DiscountCode);

                    return discount;
                }
                catch (Exception e)
                {
                    _logger.FatalException("CreateWsp_renewal: ", e);
                    throw;
                }
            }

            // we are not refilling WSP, create a new one
            Discount wspDiscount = new Discount
            {
                DiscountType = DiscountType.Subscription,
                DateValidTo = order.OrderDate,
                DateValidFrom = order.OrderDate,
                Cost = row.RowPrice,
                DateBilled = DateTime.UtcNow,
                DateVerified = DateTime.UtcNow,
                DiscountCode = whichTier + order.idOrder,
                FlatOff = 0,
                Notes = "V3 entry",
                PercentOff = 100,
                RenewalTerm = 0,
                Status = "Active",
                TotalCount = totalCount,
                idAffiliate = order.idAffiliate,
                idDiscount = order.idOrder
            };
            try
            {

                row.Discount = wspDiscount;
                SaveOrderChanges(order, null, null);

                _webUserRepository.SetWebUserWSP(row.Discount.idDiscount, order.idUser);

                FireMandrillNotificationEvent("2afda898.ttstrain.com@amer.teams.ms"
                    , "New WSP: " + order.idOrder
                    , "New WSP created for: " + wspDiscount.DiscountCode);

                return wspDiscount;
            }
            catch (Exception e)
            {
                _logger.FatalException("CreateWsp_fromNew: ", e);
                throw;
            }
        }

        //public IList<RegType> GetAllPossibleRegTypesByWebinarId(int idWebinar)
        //{

        //    IList<RegType> regTypes = GetAllPossibleRegTypesByWebinarId(idWebinar, false).Select(r => r.Key).ToList();
        //    return regTypes;

        //}

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {
            if (row.Webinar.Title == "Federal Compliance School OnDemand with Live Streaming"
                || row.Webinar.Title == "Bank Secrecy Act Seminar OnDemand with Live Streaming"
                || row.Webinar.Title.Contains("Webinar Subscription Packages")
                )
                return null;
            var thisDiscount = GetDiscountByCode(code);
            var hasRemaining = CalculateCreditsRemain(thisDiscount);

            if (!ReferenceEquals(null, thisDiscount))
            {
                if (thisDiscount.DiscountType == DiscountType.Subscription)
                {
                    if (hasRemaining >= row.RegistrationType.CreditCost)
                    {
                        //correct mismatched affiliates for WSPs
                        if (thisDiscount.idAffiliate != row.Order.Affiliate.idUserAff)
                        {
                            FireMandrillNotificationEvent(
                                "all.of.us@ttstrain.com", "Mismatched WSP Affiliate attempt is corrected on idOrder: " + row.idOrder, "WSP's affiliate is: " + thisDiscount.idAffiliate + ". Order's affiliate was: " + row.Order.idAffiliate);
                            row.Order.Affiliate = GetAffiliateById(thisDiscount.idAffiliate);
                            row.Order.idAffiliate = thisDiscount.idAffiliate;
                            _logger.Warn("ApplyDiscountCode: Mismatched WSP Affiliate attempt is corrected on idOrder: " + row.idOrder, "WSP's affiliate is: " + thisDiscount.idAffiliate + ". Order's affiliate was: " + row.Order.idAffiliate);
                        }
                        thisDiscount = RedeemDiscount(thisDiscount, row);
                        //thisDiscount.Notes += Environment.NewLine +
                        //    "Applied to: " + row.idOrder + " and used " + row.RegistrationType.CreditCost + " credits. " +
                        //    (hasRemaining - row.RegistrationType.CreditCost).ToString().Replace(".00", "") +
                        //    " will remain.";

                        row.Discount = thisDiscount;
                        _logger.Info("ApplyDiscountCode: " + row.Discount.DiscountCode + " idOrder: " + row.idOrder);
                    }
                    else
                    {
                        //thisDiscount.Notes += Environment.NewLine +
                        //    "Insufficient credits: " + hasRemaining.ToString().Replace(".00", "") + " remain but " +
                        //    row.RegistrationType.CreditCost.ToString().Replace(".00", "") + " are required.";
                        _logger.Info("ApplyDiscountCode failed due to insufficient credits: " +
                                     thisDiscount.DiscountCode + " idOrder: " + row.idOrder + +hasRemaining +
                                     " remain but " + row.RegistrationType.CreditCost + " are required.");
                    }
                    return thisDiscount;
                }

                thisDiscount = RedeemDiscount(thisDiscount, row);
                row.Discount = thisDiscount;
                _logger.Info("ApplyDiscountCode: " + JsonConvert.SerializeObject(thisDiscount));
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
            if (row.idWebinar == 2025)
                return null;
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
                case DiscountType.TwoPart:
                //case DiscountType.Package:
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
                discount.Notes = ("This Discount Code " + discount.DiscountCode + " is expired.<br> For more info contact us by using the <i>Help & Feedback</i> button below<br>.");
            }

            //if (discount.DiscountType == DiscountType.Subscription && discount.DateValidFrom > row.Order.OrderDate)
            //{
            //    _logger.Warn("Discount error: webinar took place prior to activation " + discount.idDiscount + " - " + row.idOrder);
            //    discount.Notes = ("This webinar took place prior to activation of " + discount.DiscountCode + ".<br> For more info contact us by using the <i>Help & Feedback</i> button below<br>.");
            //}

            //if (discount.DiscountType == DiscountType.Subscription && discount.DateValidTo < row.Order.OrderDate)
            //{
            //    _logger.Warn("Discount error: webinar takes place prior to activation " + discount.idDiscount + " - " + row.idOrder);
            //    discount.Notes = ("This webinar is scheduled to take place after the expiration of this Subscription Package: " + discount.DiscountCode + ".<br> For more info contact us by using the <i>Help & Feedback</i> button below<br>.");
            //}


            var forNotes = new StringBuilder();
            decimal creditsRemain = CalculateCreditsRemain(discount);
            //decimal creditsUsed = CalculateCreditsUsed(discount);
            //var existingDiscount = discount;
            //var regTypeLabel = GetRegTypeOfOrderRow(row.idRegType).OptionLabel;

            decimal thisUseCost = GetRegTypeOfOrderRow(row.idRegType).CreditCost;

            decimal thisUseCostAddLocs = 0m;

            if (row.AdditionalLocation != null && row.AdditionalLocation.Count > 0)
            {
                thisUseCostAddLocs = (decimal)row.AdditionalLocation.Count * .25m;
                thisUseCost += thisUseCostAddLocs;
            }

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
                                " If applied to this order, {0} {1} will be deducted from your package with {2} remaining.",
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
                            || (userDiscount.DateVerified > Convert.ToDateTime("09/15/17"))
                            && (o.OrderStatus == OrderStatus.Submitted || o.OrderStatus == OrderStatus.Billed || o.OrderStatus == OrderStatus.Paid || o.OrderStatus == OrderStatus.OutstandingBalance)
                            && !o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).Webinar.Title.StartsWith("Compliance Perspe"));

            var creditsUsed = 0M;

            if (ordersWithDiscount.Any())
                foreach (var order in ordersWithDiscount)
                {

                    creditsUsed +=
                        order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                            .RegistrationType.CreditCost;

                    if (order.OrderDate > DateTime.Parse("09/05/2017"))
                    {
                        var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                        if (row.AdditionalLocation != null && row.AdditionalLocation.Count > 0)
                        {
                            var thisUseCostAddLocs = (decimal)row.AdditionalLocation.Count * .25m;
                            creditsUsed += thisUseCostAddLocs;
                        }
                    }
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

                    if (order.OrderDate > DateTime.Parse("09/05/2017"))
                    {
                        var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                        if (row.AdditionalLocation != null && row.AdditionalLocation.Count > 0)
                        {
                            var thisUseCostAddLocs = (decimal)row.AdditionalLocation.Count * .25m;
                            credits += thisUseCostAddLocs;
                        }
                    }
                }

            return credits;
        }



        public int CheckIfEmailAlreadyRegisteredForWebinar(int idWebinar, string email)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.CheckIfEmailAlreadyRegisteredForWebinar(email, idWebinar);
        }


        public int CheckIfEmailAlreadyRegisteredForWebinarByDomain(int idWebinar, string email)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.CheckIfEmailAlreadyRegisteredForWebinarByDomain(email, idWebinar);
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

        public IList<Order> GetOrdersByDomain(string searchTerm, int affiliateId, out int totalNumberOrders)
        {

            var orders = _orderRepository.GetOrdersByDomain(searchTerm, affiliateId);
            totalNumberOrders = orders.Count();
            //foreach (var order in orders)
            //{
            //    order.Institution = _
            //}
            return orders;
        }

        //public string SetOnDemandClaimById(int myRowIdOrder)
        //{
        //    //RedirectResult("Account", "Signin");
        //    return null;
        //}

        public string OrderHasCc(Order order)
        {
            IList<string> ccEmailAddresses = null;
            if (string.IsNullOrWhiteSpace(order.UserComments))
                return null;
            if (!order.UserComments.Contains(JsonPropertyKeys.CarbonCopy))
                return null;

            try
            {

                var addresses = JToken.Parse(order.UserComments);
                var isCC = "";
                foreach (JProperty prop in addresses.Children<JObject>()
                    .SelectMany(content => content.Properties()
                    .Where(prop => prop.Name == JsonPropertyKeys.CarbonCopy)))
                {
                    isCC = prop.Value.ToString();
                }

                if (isCC != "")
                {
                    ccEmailAddresses = EventHandlerHelpers.GetCcEmailAddresses(isCC);
                }
                else
                {
                    var _addresses = JObject.Parse(order.UserComments)
                        .GetValue(JsonPropertyKeys.CarbonCopy).Value<string>();
                    ccEmailAddresses = EventHandlerHelpers.GetCcEmailAddresses(_addresses);
                }

                return string.Join(";", ccEmailAddresses);
            }
            catch (Exception e)
            {
                return null;
            }



        }

        public Order UserHasPrexistingOrder(Order existingOrder)
        {
            if (existingOrder == null) return null;

            var row = existingOrder.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            try
            {
                Debug.Assert(row != null, "row != null");
                var foundByEmail = _orderRepository.GetOrdersByUserId(existingOrder.idUser)
                    .Where(o => o.BillingEmail == existingOrder.BillingEmail
                    && row.idWebinar == o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar)
                    .Where(o => o.idOrder != existingOrder.idOrder)
                    ;

                var byEmail = foundByEmail as IList<Order> ?? foundByEmail.ToList();
                if (byEmail.Any())
                {
                    foreach (var order in byEmail)
                    {
                        if (order.OrderStatus == OrderStatus.Paid
                            || order.OrderStatus == OrderStatus.Billed
                            || order.OrderStatus == OrderStatus.Submitted)
                        {
                            RemoveDupedOrders(order);
                            return order;
                        }
                    }

                    RemoveDupedOrders(byEmail.First());
                    _logger.Warn("UserHasPrexistingOrder: found:" + string.Join(",", byEmail.Select(o => o.idOrder)));
                    return byEmail.First();
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.FatalException("UserHasPrexistingOrder: ", e);
                throw;
            }

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
            var api = new RegistrantsApi();

            try
            {
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
                _logger.ErrorException(string.Format("CreateRegistrantKey WebException: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, webException.Message), webException);
                var httpWebResponse = webException.Response as HttpWebResponse;

                if (!ReferenceEquals(httpWebResponse, null))
                {
                    string responsePayload = ProcessErrorByStatusCode(httpWebResponse);

                    _logger.Error("CreateRegistrantKey Citrix Message: {0}", responsePayload);

                }
                return new Registrant { firstName = webException.Message };
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("CreateRegistrantKeyException: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, exception.Message), exception);
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

    public class OrderSubmittedV2Message
    {
    }

    public class OrderSubmittedV2Event<T>
    {
    }
}
