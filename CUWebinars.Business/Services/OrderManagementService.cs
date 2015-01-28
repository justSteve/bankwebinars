using System.Configuration;
using System.Diagnostics;
using System.Linq.Expressions;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Cache;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Migrations;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using CUWebinars.Web.Core.Cache;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;
using IEventSource = CUWebinars.NotificationSystem.Event.IEventSource;

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
        private readonly TtsConfiguration _ttsConfig;
        readonly List<IEvent> _events = new List<IEvent>();
        private bool _disposed;

        public OrderManagementService(
            IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
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
            _additionalLocationsRepository = additionalLocationsRepository;
            _logger = logger;
            _webUserRepository = webUserRepository;
            _cachingService = new OrderCachingService();
        }

        public Order AssignAffiliateToOrder(Affiliate affiliate, Order order)
        {
            return _orderRepository.AssignAffiliate(affiliate, order);
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

        //TODO: remove if nothing bad happens by it's absence.
        //public string BuildConnectionInfo(OrderRow orderRow)
        //{
        //    // add code here to build string

        //    //var option = _RegTypeRepository.FindRegType(orderRow.RegistrationType);

        //    return string.Empty;
        //}

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType)
        {
            try
            {
                var regType = _regTypeRepository.FindRegType(registrationType);
                return _orderRepository.CreateOrderRow(webinar, additionalLocation, regType);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("CreateOrderRow method", exception);

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
                _logger.ErrorException("CreateOrderRow  method", exception);
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

        public Tuple<string, decimal> GetCostOfAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations, int idWebinar)
        {
            //so renamed to reflect that we are building the cost of a user's list of added seats. Not
            // how much does it cost per seat.
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var addresses = new StringBuilder();
            var i = 0;

            var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(idWebinar);

            decimal optionsCost = 0M;

            // perf tweak - ensures enumerable will only be enumerated once
            var additionalLocationsEnumerated = additionalLocations as AdditionalLocation[] ?? additionalLocations.ToArray();

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

            Debug.Assert(additionalLocationsPricing.Count == 1, "There should only ever be 1 value returned for the cost of an Additionalocation for a particular Webinar");
            // Item2 of the tuple is the price value as a decimal. Item 1 is the AdditionalLocationsLookupPrice id 
            optionsCost = additionalLocationsPricing.Single().Item2;

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
            return _affiliateRepository.FindByIdWithIncluding(id, includeProperties);
        }


        public IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached)
        {
            string cachKey = "options-" + id;
            var options = _cachingService.Get(cachKey);

            //if (options == null)
            //{
            options = _regTypeRepository.FindRegTypesByWebinarId(id, false);

            //    // keeps options object in cache for 1 hour.
            //    _cachingService.Add(cachKey, options, DateTime.Now.AddHours(1));
            //}

            return (IDictionary<RegType, bool>)options;
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

        //TODO: Review Naming convention 
        //- in my frame of reference, as relates to 'RegType' and 'Option' - they are synonyms 
        //  and to use both in a name seems redundant. --not nitpicking on names just probing for 
        //  a mismatch between our frames of reference.
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
            var order = _orderRepository.FindOrderByIdWithOrderRows(id);
            return order;
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
                throw;
            }
        }
        public WebUser GetWebUser(int id)
        {
            return _webUserRepository.FindByIdLoaded(id);
        }

        public IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar)
        {
            return _webUserRepository.GetWebusersForLiveNotifications(idWebinar);
        }
        //public IList<AdditionalLocation> GetAdditionalLocations(int idOrder)
        //{
        //    return _orderRepository.GetAdditionalLocations(idOrder);
        //}
        public void DeleteOrder(int orderId)
        {
            _orderRepository.DeleteOrder(orderId);
        }

        public Affiliate DetermineAffiliateByAlternativeMeans(int idUser)
        {
            string cachKey = "webUserId-" + idUser;
            var affiliateIds = _cachingService.Get(cachKey) as IList<int>;

            if (affiliateIds == null)
            {
                affiliateIds = _orderRepository.FindOrdersByUserId(idUser)
                                    .OrderByDescending(o => o.OrderDate)
                                    .Select(o => o.idAffiliate)
                                    .ToList();

                // keeps affiliateIds object in cache for 1 hour.
                _cachingService.Add(cachKey, affiliateIds, DateTime.Now.AddHours(1));
            }

            if (affiliateIds.Any())
            {
                //  get the most recent
                int affiliateIdForOrder, mostRecentAffiliateId;
                affiliateIdForOrder = mostRecentAffiliateId = affiliateIds.First();

                if (affiliateIds.Distinct().Count() > 1) // The history is of more than 1 affiliate
                {
                    // The business rule is that where there is more than one Affiliate which the 
                    // user has made orders for, if one affiliate has been used twice as many times 
                    // as the most recent Affiliate, then make the order for that Affiliate.
                    var groups = affiliateIds.GroupBy(a => a);
                    //TODO: Express groups in a string seperated by comma for logging purposes.
                    int mostUsedAffiliateId = 0;
                    int mostUses = 0;
                    int numberOfUsesOfMostRecentAffiliate = 0;
                    int current = 0;

                    foreach (var group in groups)
                    {
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
                        affiliateIdForOrder = mostUsedAffiliateId;

                    _logger.Error("Total affiliates considered: {\" +[convert groups to comma delim list + } for idUser = " + idUser);
                }

                cachKey = "affiliateId-" + affiliateIdForOrder;
                var affiliate = _cachingService.Get(cachKey) as Affiliate;

                if (affiliate == null)
                {
                    affiliate = _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder, a => a.WebUser); // use the most recent
                    // keeps Affiliate object in cache for 1 hour.
                    _cachingService.Add(cachKey, affiliate, DateTime.Now.AddHours(1));
                }

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
            //how is default determined? [dar] It is same as writing PricesAndDiscounts pricesAndDiscounts; For int, it would be 'int i = 0';
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            decimal totalOptionsPrice = 0M;

            var row = order.OrderRows.SingleOrDefault(orderRow => orderRow.RowStatus == OrderRowStatus.Active);

            Debug.Assert(row != null, "OrderRow object should always have a value here.");
            row.UnitPrice = (decimal)row.RegistrationType.Price;

            //Calculate row price before discount
            if (row.AdditionalLocation != null)
            {
                totalOptionsPrice = row.AdditionalLocation.Count * optionsCost; // cost * number of additional locations
            }

            row.RowPrice = row.UnitPrice + totalOptionsPrice;
            pricesAndDiscounts.UnitPrice = row.UnitPrice;

            //Calculate discount. 
            decimal discountTotal = 0;

            if (row.Discount != null && row.Discount.PercentOff != 0.0M)
            {
                discountTotal = row.RowPrice * row.Discount.PercentOff / 100;
            }
            else if (row.Discount != null && row.Discount.FlatOff != 0.0M)
            {
                discountTotal = row.Discount.FlatOff;
            }

            if (discountTotal > row.RowPrice)
            {
                discountTotal = row.RowPrice;
            }

            row.RowPrice -= discountTotal;
            pricesAndDiscounts.TotalDiscount = discountTotal;
            pricesAndDiscounts.TotalCostOfOptions = totalOptionsPrice;

            //Calculate order total
            order.Total = row.RowPrice;
            pricesAndDiscounts.Discount = row.Discount;
            pricesAndDiscounts.TotalOrderPrice = order.Total;

            return pricesAndDiscounts;
        }

        //public virtual decimal CalculateOptionsPrice(OrderRow row)
        //{
        //    ////if (row.RegistrationType == RegistrationType.SubscriptionRedeem)
        //    ////{
        //    ////    return 0.0M;
        //    ////}

        //    //var additionalLocationsPricingForWebinar = GetCostOfAdditionalLocations(row.idWebinar);

        //    //decimal optionsTotal = 0.0M;

        //    //if (row.AdditionalLocation != null)
        //    //{
        //    //    foreach (var addLoc in row.AdditionalLocation)
        //    //    {
        //    //        // Item2 of the tuple is price
        //    //        addLoc.Price = additionalLocationsPricingForWebinar[0].Item2;
        //    //        optionsTotal += addLoc.Price;
        //    //    }
        //    //}

        //    //return optionsTotal;
        //}

        public void CreateCPSubscription(OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, Uri url = null)
        {
            string addPasswordUrl = string.Empty;

            _logger.Info("Adding Event for Order {0}", order.idOrder);

            var orderSubmittedViewModel = new OrderSubmittedViewModel
            {
                AddPasswordUrl = string.Empty,
                ConfirmChangeEmailUrl = string.Empty,
                Order = order,
                OrderGenesis = userCreatedInCart ? OrderGenesis.CreatedViaCartByNewUser : OrderGenesis.CreatedViaCartByExistingUser,
                UserCreatedInCart = userCreatedInCart,
                UserCreatedOnImport = false
            };

            if (!ReferenceEquals(null, url))
            {
                var baseUri = new Uri(string.Concat(url.Scheme, @"://", url.Authority), UriKind.Absolute);
                addPasswordUrl = new Uri(
                    baseUri,
                    string.Concat(@"Account/AddPasswordForCartCreatedUser/", order.WebUser.email)
                    ).ToString();
            }

            AddEvent(new OrderSubmittedEvent<OrderSubmittedViewModel>
            {
                EventObject = orderSubmittedViewModel,
                RelativePath = addPasswordUrl
            });

            foreach (var evt in GetEvents().OfType<OrderSubmittedEvent<OrderSubmittedViewModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear(); // need to clear at this point, otherwise the OrderSubmittedEvent will be fired again when 

            if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count != 0)
            {
                foreach (var addLoc in order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                {
                    FireOrderSubmittedAdditionalLocationEvent(order, addLoc.Email);
                }
            }
        }

        public void FireEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients)
        {
            AddEvent(new EmailOrderEvent<Order> { EventObject = order, Recipients = recipients });


            foreach (var evt in GetEvents().OfType<EmailOrderEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public void FireOrderSubmittedAdditionalLocationEvent(Order order, string address)
        {
            _logger.Info("Adding Event for Order with Additional Location {0} - {1}", order.idOrder, address);

            var orderSubmittedAdditionalLocationViewModel = new OrderSubmittedAdditionalLocationViewModel
            {
                ConfirmChangeEmailUrl = string.Empty,
                Order = order,
                UserCreatedOnImport = false,
                notifyAddress = address
            };

            var relativePath = Path.Combine(@"App_Data\Notifications", string.Format("OrderNotificationAddLoc-{0}{1}", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm"));

            AddEvent(new OrderSubmittedAdditionalLocationEvent<OrderSubmittedAdditionalLocationViewModel>
            {
                EventObject = orderSubmittedAdditionalLocationViewModel,
                RelativeFilePath = relativePath
            });

            _logger.Info("Persisted Email for Order w/AddLoc {0}-{1} :{2}", order.idOrder, address, relativePath);

            foreach (var evt in GetEvents().OfType<OrderSubmittedAdditionalLocationEvent<OrderSubmittedAdditionalLocationViewModel>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public void FireSendRecordingIsPostedEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendRecordingPostedEvent<Order> { EventObject = order });
            }

            foreach (var evt in GetEvents().OfType<SendRecordingPostedEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public void FireSendReminderNotificationEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendReminderEvent<Order> { EventObject = order });
            }

            foreach (var evt in GetEvents().OfType<SendReminderEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public void FireSendConnectionInfoNotificationEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendConnectionInfoEvent<Order> { EventObject = order });
            }


            foreach (var evt in GetEvents().OfType<SendConnectionInfoEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
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
        }

        public void FireSendRecordingPostedNotificationEvent(IList<Order> orders)
        {
            foreach (var order in orders)
            {
                AddEvent(new SendRecordingPostedEvent<Order> { EventObject = order });
            }


            foreach (var evt in GetEvents().OfType<SendRecordingPostedEvent<Order>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public string UpdateOrderChanges(Order currentOrder, ref PricesAndDiscounts pricesAndDiscounts)
        {
            try
            {
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(
                    currentOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar
                    );

                pricesAndDiscounts = CalculateOrderCost(currentOrder, additionalLocationsPricing.Single().Item2);

                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, (int)currentOrder.OrderStatus);


                _logger.Info("Adding Event for Order {0}", currentOrder.idOrder);

                Clear();
                return "success";
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("SaveOrderChanges method: {0}", exception.Message), exception);
            }
            return "failed";
        }

        public void UpdateOrderWithUserEmail(int orderId, string email)
        {
            var order = GetOrderById(orderId);
            order.BillingEmail = email;

            var updatedOrder = _orderRepository.SaveOrderChanges(order, null);
        }

        public void UpdateOrderWithUserId(int orderId, int userId)
        {
            var user = _webUserRepository.FindByIdLoaded(userId);
            var order = _orderRepository.FindById(orderId);
            order.AuditInfo = "anon user is updated with " + user.email + Environment.NewLine + order.AuditInfo;
            order.idUser = userId;

            var billingAddress = user.Addresses.Where(a => a.AddressType == DomainConstants.BillingAddress).Single();
            var shippingAddress = user.Addresses.Where(a => a.AddressType == DomainConstants.ShippingAddress).Single();

            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.Institution = user.Institution.InstitutionName;
            order.BillingPhone = billingAddress.Phone;
            order.BillingEmail = user.email;
            order.BillingAddress = billingAddress.StreetAddress;
            order.BillingAddress2 = billingAddress.StreetAddress2;
            order.BillingState = billingAddress.State;
            order.BillingCity = billingAddress.City;
            order.BillingZip = billingAddress.Zip;

            order.ShippingFirstName = user.FirstName;
            order.ShippingLastName = user.LastName;
            order.ShippingPhone = shippingAddress.Phone;
            order.ShippingAddress = shippingAddress.StreetAddress;
            order.ShippingAddress2 = shippingAddress.StreetAddress2;
            order.ShippingState = shippingAddress.State;
            order.ShippingCity = shippingAddress.City;
            order.ShippingZip = shippingAddress.Zip;


            _orderRepository.SaveOrderChanges(order, null);
        }

        public void RemoveAdditionalLocationsForOrder(int idOrderRow)
        {
            try
            {
                _additionalLocationsRepository.DeleteAdditionalLocationsByOrderRowId(idOrderRow);

                var orderRow = _orderRepository.GetOrderRowById(idOrderRow);
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
                var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(orderRow.idWebinar);

                var totalOptionsDeletedCost = additionalLocationsPricing.First().Item2 * orderRow.AdditionalLocation.Count;
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

        public decimal GetPriceOfAdditionalLocation(int idWebinar)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var addLocPrice = dataOperations.GetAdditionalLocationsPricing(idWebinar);
            var tuple = addLocPrice.SingleOrDefault();

            if (tuple != null)
            {
                return tuple.Item2;
            }
            else
            {
                _logger.Warn("AdditionalLocation price not set for: {0}", idWebinar);
                return 0;
            }
        }

        public void UpdateOrderByAdmin(Order order)
        {
            var updatedOrder = _orderRepository.SaveOrderChanges(order, 0);
        }

        public Discount GetDiscountByUser(WebUser currentUser)
        {
            var myDiscount = _orderRepository.FindDiscountByUser(currentUser);
            return myDiscount;
        }

        public void GenerateRegistrantKey(Order order, AdditionalLocation additionalLocation)
        {
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var regKeyResponse = string.Empty;
            if (additionalLocation.Email == null)
            {
                if (row.JoinURL == null && row.RegistrationType.ShowLiveNotifications.TrimEnd().Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    regKeyResponse = CreateRegistrantKey(order.FirstName, order.LastName
                        , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);
                }
            }
            else
            {
                regKeyResponse = CreateRegistrantKey("CareOf",//DomainConstants.CareOfString,
                    order.LastName, additionalLocation.Email, row.Webinar.idWebinar, row.Webinar.WebinarKey);
            }

            JObject parsedJsonObject;

            if (ReferenceEquals(null, regKeyResponse))
            {
                //throw new NullReferenceException(
                //    "The Registration Key Response from the Citrix API resulted in a null response.");
                _logger.FatalException("The Registration Key creation failed.", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response."));

            }
            else
            {
                parsedJsonObject = JObject.Parse(regKeyResponse);

                if (parsedJsonObject[DomainConstants.RegistrantKey] != null)
                {
                    _logger.Info("RegKey for ." + order.idOrder + " = " + regKeyResponse);

                    var registrantKey = parsedJsonObject[DomainConstants.RegistrantKey].ToString();
                    var joinUrl = parsedJsonObject[DomainConstants.JoinUrl].ToString();
                    if (additionalLocation.Email == null)
                    {
                        row.RegistrantKey = registrantKey;
                        row.JoinURL = joinUrl;
                    }
                    else
                    {
                        additionalLocation.JoinURL = joinUrl;
                        additionalLocation.RegistrantKey = registrantKey;
                    }

                    var pricesAndDiscounts = default(PricesAndDiscounts); // not needed here. Just used b/c ref parameter required below.

                    var resultOfUpdate = UpdateOrderChanges(order, ref pricesAndDiscounts);
                }
            }
        }


        public void GetJoinUrl(OrderRow row)
        {
            Order order = row.Order;


            if (row.JoinURL == null && row.RegistrationType.ShowLiveNotifications == "Yes")
            {
                var regKeyResponse = CreateRegistrantKey(order.FirstName, order.LastName
                    , order.BillingEmail, row.Webinar.idWebinar, row.Webinar.WebinarKey);

                if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count > 0)
                {
                    foreach (var additionalLocation in order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                    {
                        if (
                            string.IsNullOrEmpty(
                                order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).JoinURL))
                        {
                            //Blows up where with 
                            GenerateRegistrantKey(order, additionalLocation); 
                        }
                    }
                }

                if (ReferenceEquals(null, regKeyResponse))
                {
                    _logger.ErrorException(
                        "CreateRegistrantKey failed on Webinar: " + row.Webinar.idWebinar + " email: " +
                        order.BillingEmail,
                        new NullReferenceException("Attempt to CreateRegistrantKey failed on Webinar: {0} email: {1}" +
                                                   row.Webinar.idWebinar + " email: " + order.BillingEmail));
                }
                else
                {

                    JObject parsedJsonObject = JObject.Parse(regKeyResponse);
                    if (parsedJsonObject[DomainConstants.RegistrantKey] != null)
                    {
                        _logger.Error("Successful CreateRegistrantKey");
                        var registrantKey = parsedJsonObject[DomainConstants.RegistrantKey].ToString();
                        var joinUrl = parsedJsonObject[DomainConstants.JoinUrl].ToString();

                        row.RegistrantKey = registrantKey;
                        row.JoinURL = joinUrl;
                    }
                    else
                    {
                        _logger.Error("ERROR at CreateRegistrantKey on " + row.Order.idOrder);
                    }
                }
            }
        }

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {
            var thisDiscount = GetDiscountByCode(code);

            if (!ReferenceEquals(null, thisDiscount))
            {
                row.Discount = thisDiscount;
                RedeemDiscount(thisDiscount);
            }

            return thisDiscount;
        }

        public Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink, OrderGenesis orderGenesis = OrderGenesis.ImportedForExistingUser)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(currentOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar);
            var tuple = additionalLocationsPricing.SingleOrDefault();
            decimal optionsPrice = 0M;

            // If no data is stored for AdditionalLocations pricing in db, price will be $0. Up to us to ensure pricing is available.
            if (!ReferenceEquals(null, tuple))
                optionsPrice = tuple.Item2;

            ProcessDiscountCodes(currentOrder);
            CalculateOrderCost(currentOrder, optionsPrice);

            try
            {
                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, 0);

                // If linkToVerifyAccount is true, then we know that the user was created during an importation. When that occurs, 
                // we don't want to send the normal register user email. We want to roll those details into this confirmation
                // notification (OrderSubmitted notification). 
                //  TODO: verify that this condition is still operative and point to an instance where used?
                //So, if we have the confirmChangeEmailLink, we can include it in 
                // the notification confirming registration for this webinar.  
                bool linkToVerifyAccount = !string.IsNullOrWhiteSpace(confirmChangeEmailLink);

                _logger.Info("Adding Event for Order {0}", currentOrder.idOrder);


                var orderSubmittedViewModel = new OrderSubmittedViewModel
                {
                    ConfirmChangeEmailUrl =
                        linkToVerifyAccount
                            ? string.Concat(confirmChangeEmailLink.Replace(DomainConstants.Blank, string.Empty),
                                currentOrder.WebUser.LastName.ToLower())
                            : string.Empty,
                    Order = updatedOrder,
                    OrderGenesis = orderGenesis,
                    UserCreatedOnImport = linkToVerifyAccount
                };


                //  The following "if" statement suppresses the OrderSubmitted notification where anonymous user has
                //  clicked the SignUp button or the order was migrated. The notification is still sent, it is just that
                //  it will be sent when the user clicks the "Bill Me" button on the 3rd tab of the cart. Not now.
                if (!currentOrder.Origin.Equals("Migrator", StringComparison.OrdinalIgnoreCase) &&
                    !currentOrder.Origin.Equals(DomainConstants.Cart, StringComparison.OrdinalIgnoreCase))
                {
                    AddEvent(new OrderSubmittedEvent<OrderSubmittedViewModel>
                    {
                        EventObject = orderSubmittedViewModel,
                        RelativePath = string.Empty
                    });
                }

                foreach (var evt in GetEvents())
                {
                    _ttsConfig.NotificationEventBus.RaiseEvent(evt);
                }

                Clear();

                return updatedOrder;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("SaveOrderChanges method: {0}", exception.Message), exception);
            }
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
            return _orderRepository.CheckUserForRecordingAccess(w, u);
        }

        private void ProcessDiscountCodes(Order order)
        {
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            //
            if (row.Discount != null)
            {
                RedeemDiscount(row.Discount);
                _logger.Info("Redeemed Discount On Order: " + order.idOrder);
            }
            else
            {
                //_logger.Error("ERROR: Rejected Discount On Order: " + order.idOrder);
            }
        }
        public virtual bool IsDiscountCodeValid(string discountCode)
        {

            return true;
        }


        private void RedeemDiscount(Discount discount)
        {
            if (discount.DiscountType != DiscountType.Subscription)

                discount.UsesCount++;

            if (discount.UsesRemain > 0)
                discount.UsesRemain--;

            _logger.Info("Discount was redeemed for {0}.", discount.DiscountCode);

        }

        private void RejectDiscount(OrderRow orderRow)
        {
            orderRow.Discount.UsesRemain++;
            //according to legacy code but can a condition exist 
            //  a non-valid discount resulted in a decrement.
        }

        //public void AddOrderRow(Order currentOrder, OrderRow orderRow)
        //{
        //    throw new NotImplementedException();
        //}

        public Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null)
        {
            var order = _orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow, origin);
            var email = webUser == null ? "notauthenticated@cuwebinars.com" : webUser.email;

            _logger.Info("CreateNewOrder: " + email + " | " + orderRow.Webinar.Title + " | " + orderRow.RegistrationType.OptionLabel);
            return order;
        }

        public Discount GetDiscountById(int id)
        {
            var discount = _orderRepository.FindDiscountById(id);
            return discount;
        }

        public string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int webinarId,
            string webinarKey)
        {
            var webinar = _webinarRepository.FindById(webinarId);
            string orgKey = webinar.OrganizerKey;
            string accessToken = webinar.OrganizerOAuthKey;

            string url = "https://api.citrixonline.com/G2W/rest/organizers/" + orgKey + "/webinars/" + webinarKey + "/registrants";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Accept = "application/vnd.citrix.g2wapi-v1.1+json";
            httpWebRequest.Headers.Add("Authorization", "OAuth oauth_token=" + accessToken);

            httpWebRequest.Method = "POST";

            object sendVars = new { firstName = firstName, lastName = lastName, email = billingEmail };

            string postData = JsonConvert.SerializeObject(sendVars);
            ;

            byte[] requestBytes = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentLength = requestBytes.Length;

            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }

            _logger.Info("CreateRegistrantKey starts: " + billingEmail + ", webinarKey = " + webinarKey + ", OrgKey = " + orgKey + ", oauth_token=" + accessToken);
            try
            {
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level 
                //stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                Console.WriteLine("Response stream received.");
                var myResponse = readStream.ReadToEnd();
                response.Close();
                readStream.Close();

                _logger.Info("CreateRegistrantKey returns: " + billingEmail + ", " + webinarKey + " - Response = " + myResponse);
                return myResponse;
            }
            catch (WebException webException)
            {
                _logger.ErrorException(string.Format("WebException CreateRegistrantKey: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, webException.Message), webException);
                var httpWebResponse = webException.Response as HttpWebResponse;

                if (httpWebResponse != null &&
                    (httpWebResponse.StatusCode == HttpStatusCode.Conflict ||
                     httpWebResponse.StatusCode == HttpStatusCode.NotFound))
                {
                    string responsePayload = string.Empty;

                    using (var responsStream = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        responsePayload = responsStream.ReadToEnd();
                    }

                    return responsePayload;
                }
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Exception CreateRegistrantKey: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, exception.Message), exception);
                return "error";
            }
            return null;
        }


        public void Clear()
        {
            _events.Clear();
        }
        public static string CreateCalendarEvent(string title, string body, DateTime startDate, double duration, string location, string organizer, string eventId, bool allDayEvent)
        {
            // mandatory for outlook 2007
            if (String.IsNullOrEmpty(organizer))
                throw new Exception("Organizer provided was null");

            var iCal = new iCalendar
            {
                Method = "PUBLISH",
                Version = "2.0"
            };

            // "REQUEST" will update an existing event with the same UID (Unique ID) and a newer time stamp.
            //if (updatePreviousEvent)
            //{
            //    iCal.Method = "REQUEST";
            //}

            var evt = iCal.Create<Event>();
            evt.Summary = title;
            evt.Start = new iCalDateTime(startDate);
            evt.Duration = TimeSpan.FromHours(duration);
            evt.Description = body;
            evt.Location = location;
            evt.IsAllDay = allDayEvent;
            evt.UID = String.IsNullOrEmpty(eventId) ? new Guid().ToString() : eventId;
            evt.Organizer = new Organizer(organizer);
            evt.Alarms.Add(new Alarm
            {
                Duration = new TimeSpan(0, 15, 0),
                Trigger = new Trigger(new TimeSpan(0, 15, 0)),
                Action = AlarmAction.Display,
                Description = "Reminder"
            });

            return new iCalendarSerializer().SerializeToString(iCal);
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
