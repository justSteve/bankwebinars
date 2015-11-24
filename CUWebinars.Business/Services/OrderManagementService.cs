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
using System.Web.UI.WebControls;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Core.Helpers;
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

        public void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation)
        {
            _orderRepository.AddAdditionalLocation(addedAdditionalLocation);
        }

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

        public Order CreateNewOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null)
        {
            var order = _orderRepository.CreateOrder(affiliateId, webUser, webinar, orderRow, origin);
            var email = webUser == null ? "notauthenticated@cuwebinars.com" : webUser.email;

            //_logger.Info("CreateNewOrder: " + email + " | " + orderRow.Webinar.Title + " | " + orderRow.RegistrationType.OptionLabel);
            return order;

        }

        public OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType)
        {
            try
            {
                var regType = _regTypeRepository.FindRegType(registrationType);
                return _orderRepository.CreateOrderRow(webinar, additionalLocation, regType);
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
            optionsCost = additionalLocationsPricing.Single().Price;

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
            var aff = _affiliateRepository.FindByIdWithIncluding(id, includeProperties);
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
            var options = _cachingService.Get(cachKey);

            //if (options == null)
            //{
            options = _regTypeRepository.FindRegTypesByWebinarId(id, false);

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
            return _orderRepository.FindExpressCheckoutOrder(email.Trim(), idWebinar);
        }

        public IEnumerable<Order> GetOrdersByEmailDomain(string email, int aff)
        {
            return _orderRepository.FindOrdersByBillingEmailDomain(email.Trim(), aff);
        }

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

        public OrderRow GetLegacyOrder(Order order)
        {
            var dataOperations = new DataOperations(TtsConfig.LegacyConnectionString);
            return dataOperations.GetLegacyOrder(order);

        }

        public void SynchOrders(int webinarId)
        {
            _logger.Info("SynchOrders begins for: " + webinarId);
            var dataOperations = new DataOperations(TtsConfig.LegacyConnectionString);
            IList<Order> lOrders = dataOperations.GetLegacyOrdersByWebinar(webinarId);
            IList<Order> v3Orders = GetV3OrdersByWebinar(webinarId);


            List<string> legacyEmails = lOrders.Select(order => order.BillingEmail).ToList();
            List<string> v3Emails = v3Orders.Select(v3Order => v3Order.BillingEmail).ToList();


            var query = legacyEmails.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();
            foreach (var d in query)
            {
                _logger.Warn("SynchError: Duped order from Legacy " + webinarId + " on " + d);
            }


            var queryv3 = v3Emails.GroupBy(x => x)
                  .Where(g => g.Count() > 1)
                  .Select(y => y.Key)
                  .ToList();
            foreach (var d in query)
            {
                _logger.Warn("SynchError: Duped order from V3 " + webinarId + " on " + d);
            }


            // find orders in legacy not in v3
            var missingFromV3 = legacyEmails.Except(v3Emails).ToList();

            // find orders in v3Emails not in Legacy
            var missingFromLegacy = v3Emails.Except(legacyEmails).ToList();

            // find common numbers in both arrays
            var commonEmails = legacyEmails.Intersect(v3Emails).ToList();

            _logger.Info("SynchOrders on {3} found {0} missingFromV3, {1} missingFromLegacy, and {2} already synched. ", missingFromV3.Count(), missingFromLegacy.Count(), commonEmails.Count(), webinarId);
            var i = 1;
            try
            {
                if (missingFromLegacy.Count > 0)
                    foreach (var orderEmail in missingFromLegacy)
                    {
                        var order = v3Orders.SingleOrDefault(o => o.BillingEmail == orderEmail);
                        if (order != null && order.WebUser.email.EndsWith("notauthenticated.com"))
                        {
                            var user = _webUserRepository.GetWebUserByEmail(orderEmail);
                            if (user == null)
                            {
                                user = _webUserRepository.BuildPlaceHolderUser(orderEmail);
                                order.WebUser = user;
                            }
                            else
                            {
                                order.WebUser = user;
                            }
                            order.FirstName = user.FirstName;
                            order.LastName = user.LastName;
                            order.BillingAddress = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").StreetAddress;
                            order.BillingAddress2 = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").StreetAddress2;
                            order.BillingCity = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").City;
                            order.BillingState = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").State;
                            order.BillingZip = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").Zip;
                            order.BillingPhone = user.Addresses.SingleOrDefault(a => a.AddressType == "Billing").Phone;

                            order.ShippingAddress = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").StreetAddress;
                            order.ShippingAddress2 = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").StreetAddress2;
                            order.ShippingCity = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").City;
                            order.ShippingState = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").State;
                            order.ShippingZip = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").Zip;
                            order.ShippingPhone = user.Addresses.SingleOrDefault(a => a.AddressType == "Shipping").Phone;
                            try
                            {
                                SaveOrderChanges(order, "", "", OrderGenesis.CreatedViaCartByExistingUser);
                                _logger.Info("SynchOrder added missing address for: ", orderEmail);

                            }
                            catch (Exception ex)
                            {

                                _logger.FatalException("SynchOrder failed to add address for " + orderEmail, ex);
                            }

                        }
                        try
                        {
                            _orderRepository.MigrateOrderFromV3(order);
                            _logger.Info("SynchOrder MigrateToLegacy {0} of {1} - {2}", i, missingFromLegacy.Count(), orderEmail);
                            i++;
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorException("SynchOrder to Legacy Failed: " + orderEmail, ex);
                        }
                    }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("missingFromLegacy loop failed.  i=" + i + " idWebinar=" + webinarId, ex);
            }

            //Missing From V3
            i = 1;
            try
            {
                if (missingFromV3.Count > 0)
                    foreach (var orderEmail in missingFromV3)
                    {
                        var order = lOrders.SingleOrDefault(o => o.BillingEmail == orderEmail);
                        try
                        {
                            _orderRepository.ConvertLegacyOrder(order);
                            _logger.Info("SynchOrder MigrateToV3 {0} of {1} - {2}", i, missingFromV3.Count(), orderEmail);

                        }
                        catch (Exception ex)
                        {
                            i++;
                            _logger.ErrorException("SynchOrder Migrate to V3 failed: " + orderEmail, ex);
                        }
                        try
                        {
                            _orderRepository.MigrateOrderWithDiscount(order);
                            _logger.Info("SynchOrder Discount {0} of {1} - {2}", i, missingFromV3.Count(), orderEmail);

                        }
                        catch (Exception ex)
                        {
                            i++;
                            _logger.ErrorException("SynchOrder Migrate to V3 failed: " + orderEmail, ex);
                        }
                        i++;

                    }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("missingFromV3 loop failed. i=" + i + " idWebinar=" + webinarId, ex);
            }

            i = 1;
            try
            {
                if (commonEmails.Count > 0)
                    foreach (var orderEmail in commonEmails)
                    {
                        var lOrder = lOrders.SingleOrDefault(o => o.BillingEmail == orderEmail);
                        var vOrder = v3Orders.SingleOrDefault(o => o.BillingEmail == orderEmail);
                        try
                        {
                            _logger.Info("SynchOrder CommonToBoth {0} of {1} - {2} ", i, commonEmails.Count(), orderEmail);
                            if (vOrder.idOrder != vOrder.idOrderLegacy)
                            {
                                _orderRepository.SynchIds(vOrder.idOrderLegacy, vOrder.idOrder);
                            }
                            if (lOrder.Total != vOrder.Total)
                            {
                                _logger.Info("SynchOrder Legacy Total = {0} vs. V3 Total = {1} on {2} ", lOrder.Total, vOrder.Total, orderEmail);
                            }
                            if (lOrder.idAffiliate != vOrder.idAffiliate)
                            {
                                _logger.Warn("SynchOrder AFFILIATE MISMATCH Legacy={0} - V3={1}", lOrder.idAffiliate, vOrder.idAffiliate);
                                //vOrder.idAffiliate = lOrder.idAffiliate;
                                _logger.Info("UPDATE dbo.[Order] SET idAffiliate = {0} WHERE idOrder = {1}", lOrder.idAffiliate, vOrder.idOrder);

                            }

                            if (lOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType != vOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType)
                            {
                                vOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType = lOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType;
                                SaveChanges();
                                _logger.Info("SynchOrder adjusted RegType from V3 RegType = {1} to Legacy = {0} on {2} ", lOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType, vOrder.OrderRows.SingleOrDefault(o => o.RowStatus == OrderRowStatus.Active).idRegType, orderEmail);

                            }


                            i++;
                        }
                        catch (Exception ex)
                        {
                            _logger.Info("EXEC dbo.FindDupeEmailPerWebinarAndRemove @idWebinar =  " + webinarId);
                            _logger.ErrorException("SynchOrder checks common orders to V3 failed: " + orderEmail, ex);
                        }
                    }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("SynchOrder CommonToBoth loop failed.  i=" + i + " idWebinar=" + webinarId, ex);
            }

            _logger.Info("SynchOrders ends for: " + webinarId);
        }

        public Order FindExpressCheckoutOrderByOrderId(int q11Orderid)
        {
            return _orderRepository.FindExpressCheckoutOrderByOrderId(q11Orderid);
        }

        public string GetAccessToRecording(Order order)
        {
            throw new NotImplementedException();
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

        public string SetPostEventClaims(int webinarId)
        {
            throw new NotImplementedException();
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
                _cachingService.Add(cachKey, affiliateIds, DomainConstants.BuildUtcNowAsCts.AddHours(1));
            }

            if (affiliateIds.Any())
            {
                //  get the most recent
                int affiliateIdForOrder, mostRecentAffiliateId;
                affiliateIdForOrder = mostRecentAffiliateId = affiliateIds.First();

                var sb = new StringBuilder();
                // The history is of more than 1 affiliate
                if (affiliateIds.Distinct().Count() > 1)
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
                        affiliateIdForOrder = mostUsedAffiliateId;

                    _logger.Error("{3} affiliates considered: {0} for idUser {1}. Credited to {2}.", sb.ToString(), idUser, affiliateIdForOrder, current);

                }

                cachKey = "affiliateId-" + affiliateIdForOrder;
                var affiliate = _cachingService.Get(cachKey) as Affiliate;

                if (affiliate == null)
                {
                    affiliate = _affiliateRepository.FindByIdWithIncluding(affiliateIdForOrder, a => a.WebUser); // use the most recent
                    if (affiliate == null)
                    {
                        affiliate = GetAffiliateById(19);
                    }
                    // keeps Affiliate object in cache for 1 hour.
                    _cachingService.Add(cachKey, affiliate, DomainConstants.BuildUtcNowAsCts.AddHours(1));
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
            //how is default determined? [dar] It is same as writing PricesAndDiscounts pricesAndDiscounts; For int, it would be 'int i = 0';
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            decimal totalOptionsPrice = 0M;

            var row = order.OrderRows.SingleOrDefault(orderRow => orderRow.RowStatus == OrderRowStatus.Active);

            Debug.Assert(row != null, "OrderRow object should always have a value here.");
            if (row.RegistrationType != null)
            {
                //TODO: Figure out why Order that are retrieved via the SynchOrders method have
                //a null reference for RegistrationType
                row.UnitPrice = (decimal)row.RegistrationType.Price;
            }
            else
            {
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

            pricesAndDiscounts.TaxAmount = 0;

            if (order.BillingState == "WI"
                && !row.RegistrationType.OptionLabel.StartsWith("Live Plus Five")
                && row.RowPrice > 0
                )
            {
                pricesAndDiscounts.TaxAmount = Convert.ToDecimal(Convert.ToInt32(row.RowPrice) * .055);
            }


            //Calculate order total
            order.Total = row.RowPrice + pricesAndDiscounts.TaxAmount;
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

        public void FireOrderSynchEvent(Order order, bool userCreatedInCart = false, bool resending = false, Uri url = null)
        {
            string addPasswordUrl = string.Empty;

            var orderSynchMessage = new OrderSynchMessage
            {
                AddPasswordUrl = string.Empty,
                ConfirmChangeEmailUrl = string.Empty,
                Details = order.NotificationStorage,
                idOrder = order.idOrder,
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
                    string.Concat(@"acc/apwd/", order.WebUser.email)
                    ).ToString();
            }

            AddEvent(new OrderSynchEvent<OrderSynchMessage>
            {
                Details = order.NotificationStorage,
                EventObject = orderSynchMessage,
                RelativePath = addPasswordUrl,
                ResendEvent = resending
            });

            foreach (var evt in GetEvents().OfType<OrderSynchEvent<OrderSynchMessage>>())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            //int rowsUpdated = _orderRepository.SaveChanges();

            Clear(); // need to clear at this point, otherwise the OrderSubmittedEvent will be fired again when 

            //if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count != 0)
            //{
            //    foreach (var addLoc in order.OrderRows
            //        .Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
            //    {
            //        FireOrderSubmittedAdditionalLocationEvent(order, addLoc.Email, resending);
            //    }
            //}

        }

        public void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false,
            Uri url = null)
        {
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
                    string.Concat(@"acc/apwd/", order.WebUser.email)
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
                    ExpiryDate = CalculatePostEventMaterialsAccessExpiry(order).ToShortDateString(),
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
                sb.Append(myRow.RegistrationType.OptionLabel);
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                if (!ReferenceEquals(myRow.Discount, null))
                {
                    decimal amountToReduce;
                    sb.Append("<tr>");
                    if (!ReferenceEquals(myRow.Discount.FlatOff, null))
                    {
                        amountToReduce = Convert.ToDecimal(myRow.Order.Total) - (myRow.Discount.FlatOff);
                        sb.Append(
                            "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                        sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                        sb.Append("            Amt. of Discount:");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                        sb.Append(
                            "    <td width='350px' style='text-align: left; color: red; background-color: #B4D1EC; padding-left: 6px;'>");
                        sb.Append(
                            "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                        sb.Append("            <b>$");
                        sb.Append(amountToReduce.ToString().Replace(".00", ""));
                        sb.Append("            </b>");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                    }
                    if (!ReferenceEquals(myRow.Discount.PercentOff, null))
                    {
                        amountToReduce = (myRow.Discount.PercentOff * 100) /
                                         Convert.ToDecimal(string.Format("{0:0.00}", myRow.Order.Total));

                        sb.Append(
                            "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                        sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                        sb.Append("            Amt. of Discount:");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                        sb.Append(
                            "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                        sb.Append(
                            "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                        sb.Append("            <b>$");
                        sb.Append(amountToReduce.ToString().Replace(".00", ""));
                        sb.Append("            </b>");
                        sb.Append("        </span>");
                        sb.Append("    </td>");
                    }
                    sb.Append("</tr>");
                }

                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            Cost:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>$");
                sb.Append(myRow.Order.Total.ToString().Replace(".00", ""));
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(
                    "    <td valign='top' width='150px' style='text-align: right; background-color: #CCCCCC; padding-right: 6px; font-family: Arial, Helvetica, sans-serif; font-size: 10px'>");
                sb.Append("        <span align='right' style='vert-align: top; font-size: 10px;'>");
                sb.Append("            OnDemand Access Expires:");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append(
                    "    <td width='350px' style='text-align: left; background-color: #B4D1EC; padding-left: 6px;'>");
                sb.Append("");
                sb.Append(
                    "        <span style='color: #000000; font-family: Arial, Helvetica, sans-serif; font-size: 12px;'>");
                sb.Append("            <b>");

                sb.Append(CalculatePostEventMaterialsAccessExpiry(order).ToShortDateString());
                sb.Append("            </b>");
                sb.Append("        </span>");
                sb.Append("    </td>");
                sb.Append("</tr>");

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

            //see SendConnectionInfoHandler for handling implementation
            foreach (var order in orders)
            {
                AddEvent(new SendConnectionInfoEvent<Order> { EventObject = order, ResendEvent = resending, Details = order.NotificationStorage });
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

        public string UpdateOrderChanges(Order currentOrder, ref PricesAndDiscounts pricesAndDiscounts)
        {
            try
            {
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

                var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(
                    currentOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar
                    );

                pricesAndDiscounts = CalculateOrderCost(currentOrder, additionalLocationsPricing.Single().Price);

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

                var totalOptionsDeletedCost = additionalLocationsPricing.First().Price * orderRow.AdditionalLocation.Count;
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

        //public Discount GetDiscountById(int discount)
        //{
        //    var myDiscount = _orderRepository.FindDiscountById(discount);
        //    return myDiscount;
        //}

        public decimal GetPriceOfAdditionalLocation(int idWebinar)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var addLocPrice = dataOperations.GetAdditionalLocationsPricing(idWebinar);
            var additionalLocationsPricing = addLocPrice.SingleOrDefault();

            if (additionalLocationsPricing.LookupPriceId > 0) // struct equivalent of checking for null.
            {
                return additionalLocationsPricing.Price;
            }

            _logger.Warn("AdditionalLocation price not set for: {0}", idWebinar);
            return 0;
        }

        public void UpdateOrderByAdmin(Order order)
        {
            var updatedOrder = _orderRepository.SaveOrderChanges(order, 0);
        }

        public int SynchExpressCheckoutOrder(Order order)
        {
            return _orderRepository.MigrateOrderFromV3(order);
            ;
        }

        public void SynchIds(Order order)
        {
            _orderRepository.SynchIds(order.idOrderLegacy, order.idOrder);
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

        public void UpdateWebUserInfo(string firstName, string LastName, string email, string institution, Address billingAddress)
        {
            var webUserIdByEmail = _webUserRepository.GetWebUserIdByEmail(email);
            if (webUserIdByEmail != null)
            {
                var orders2Update = _orderRepository.GetOrdersByUserId(webUserIdByEmail.Value);

                foreach (var order in orders2Update)
                {
                    order.FirstName = firstName;
                    order.LastName = LastName;
                    order.BillingEmail = email;
                    order.Institution = institution;
                }
            }

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

        public void GenerateRegistrantKey(Order order, AdditionalLocation additionalLocation = null)
        {
            if (order == null) throw new ArgumentNullException("order");
            return;
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            var regKeyResponse = string.Empty;

            Debug.Assert(!string.IsNullOrWhiteSpace(row.CitrixJoinUrl), "CitrixJoinUrl should always be null or empty before this method is called as a pre-condition.");

            if (additionalLocation == null)
            {
                // This branch gets key for main registrant - BillingEmail on the Order
                if (row.RegistrationType.ShowLiveNotifications.TrimEnd().Equals("Yes", StringComparison.OrdinalIgnoreCase))
                {
                    regKeyResponse = CreateRegistrantKey(
                        order.FirstName,
                        order.LastName,
                        order.BillingEmail,
                        row.Webinar.idWebinar,
                        row.Webinar.WebinarKey
                        );
                }
            }
            else
            {
                // This branch gets key for main Additional Locations
                regKeyResponse = CreateRegistrantKey(
                    "c/o " + order.FirstName,
                    order.LastName,
                    additionalLocation.Email,
                    row.Webinar.idWebinar,
                    row.Webinar.WebinarKey
                    );
            }

            if (string.IsNullOrWhiteSpace(regKeyResponse))
            {
                //throw new NullReferenceException(
                //    "The Registration Key Response from the Citrix API resulted in a null response.");
                _logger.FatalException("The Registration Key creation failed.", new NullReferenceException("The Registration Key Response from the Citrix API resulted in a null response."));

            }
            else
            {
                // If in error, there'll be no braces. In such a case, make the error a Json object.
                if (!regKeyResponse.Contains("{"))
                    regKeyResponse = string.Concat("{ \"error\": \"", regKeyResponse, "\"}");

                JObject parsedJsonObject = JObject.Parse(regKeyResponse);

                if (parsedJsonObject[DomainConstants.RegistrantKey] != null)
                {
                    _logger.Info("RegKey for ." + order.idOrder + " = " + regKeyResponse);

                    var registrantKey = parsedJsonObject[DomainConstants.RegistrantKey].ToString();
                    var joinUrl = parsedJsonObject[DomainConstants.JoinUrl].ToString();

                    if (additionalLocation == null)
                    {
                        row.RegistrantKey = registrantKey;
                        row.CitrixJoinUrl = joinUrl;
                    }
                    else
                    {
                        additionalLocation.JoinURL = joinUrl;
                        additionalLocation.RegistrantKey = registrantKey;
                    }


                    // Next variable not needed here. Just used b/c ref parameter in next method-call.
                    var pricesAndDiscounts = default(PricesAndDiscounts);

                    // Return result is actually not required. Do nothing with it, unless want to log something. Context SaveChanges is called.
                    var resultOfUpdate = UpdateOrderChanges(order, ref pricesAndDiscounts);
                }
            }
        }

        public int GetNumberOfOrdersPerWebinar(int id)
        {
            return _orderRepository.GetNumberOfOrdersPerWebinar(id);
        }

        public void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation)
        {
            _orderRepository.RemoveAndDeleteAdditionalLocation(deletedAdditionalLocation);
        }


        public DateTime CalculatePostEventMaterialsAccessExpiry(Order order)
        {

            if (order == null) throw new ArgumentNullException("order");

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // let exception be thrown if there is not a single 

            var regType = GetRegTypeOfOrderRow(orderRow.idRegType);

            //establish order date as starting point
            DateTime expryDate = order.OrderDate.AddMonths(6);

            //if order's placed before event - override starting point
            if (orderRow.Webinar.Date > order.OrderDate)
                expryDate = orderRow.Webinar.Date.AddMonths(6);


            if (regType.ShowRecordingNotifications.Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return expryDate;
            }
            // now we only addressing Live+5
            //update to pull LivePlusFive value from database

            return orderRow.Webinar.LivePlusFiveValue;
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

                if (order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation.Count > 0)
                {
                    foreach (var additionalLocation in order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation)
                    {
                        if (
                            string.IsNullOrEmpty(
                                order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).CitrixJoinUrl))
                        {
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
                    // If in error, there'll be no braces. In such a case, make the error a Json object.
                    if (!regKeyResponse.Contains("{"))
                        regKeyResponse = string.Concat("{ \"error\": \"", regKeyResponse, "\"}");


                    JObject parsedJsonObject = JObject.Parse(regKeyResponse);

                    if (parsedJsonObject[DomainConstants.RegistrantKey] != null)
                    {
                        var registrantKey = parsedJsonObject[DomainConstants.RegistrantKey].ToString();
                        var joinUrl = parsedJsonObject[DomainConstants.JoinUrl].ToString();

                        row.RegistrantKey = registrantKey;
                        row.CitrixJoinUrl = joinUrl;

                        _logger.Info("Successful CreateRegistrantKey: {0}", row.RegistrantKey);
                    }
                    else
                    {
                        if (!regKeyResponse.Contains("(409) Conflict."))
                        {
                            //409 conflict means email already registered
                            // no need to log multiple citrix hits
                            _logger.Error("ERROR at CreateRegistrantKey on " + row.Order.idOrder);
                        }
                    }
                }
            }
        }

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {

            var thisDiscount = GetDiscountByCode(code);

            if (!ReferenceEquals(null, thisDiscount))
            {
                var forNotes = new StringBuilder();
                forNotes.AppendFormat(
                    "ApplyDiscountCode: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}",
                    thisDiscount.DiscountCode, thisDiscount.DateValidFrom, thisDiscount.DateValidTo,
                    thisDiscount.CreditsUsed, thisDiscount.CreditsRemain);
                thisDiscount.Notes = forNotes.ToString();

                row.Discount = thisDiscount;

                RedeemDiscount(thisDiscount, row);

                //Adding an explicit save attempting to persist the applied discount. Could be problemic
                //SaveChanges();
            }
            _logger.Info("ApplyDiscountCode: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}", thisDiscount.DiscountCode, thisDiscount.DateValidFrom, thisDiscount.DateValidTo, thisDiscount.CreditsUsed, thisDiscount.CreditsRemain);
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
                optionsPrice = tuple.Price;

            ProcessDiscountCodes(currentOrder);
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
                    !currentOrder.Origin.Equals(DomainConstants.Cart, StringComparison.OrdinalIgnoreCase))
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

        private void ProcessDiscountCodes(Order order)
        {
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            //
            if (row.Discount != null)
            {
                RedeemDiscount(row.Discount, row);
                _logger.Info("Redeemed Discount On Order: " + order.idOrder);
            }
        }
        //public virtual bool IsDiscountCodeValid(string discountCode)
        //{

        //    return true;
        //}


        private void RedeemDiscount(Discount discount, OrderRow row)
        {
            var forNotes = new StringBuilder();

            _logger.Info("Discount: RedeemDiscountStarts: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}", discount.DiscountCode, discount.DateValidFrom, discount.DateValidTo, discount.CreditsUsed, discount.CreditsRemain);
            if (discount.DiscountType != DiscountType.Compensation && discount.DiscountType != DiscountType.ComplianceSeries)
            {
                if (discount.CreditsRemain > 0)
                {

                    discount.CreditsUsed++;
                    CalculateDiscountRedemtion(discount, row);
                    forNotes.AppendFormat(
                        "Discount Applied: {0}, validFrom: {1}, validTo: {2}, CreditedUsed: {3}, CreditsRemain: {4}"
                        , discount.DiscountCode, discount.DateValidFrom, discount.DateValidTo, discount.CreditsUsed,
                        discount.CreditsRemain);
                }
                else
                {
                    forNotes.Append("No Credits Remain. Credits used = " + discount.CreditsUsed);
                }
            }
            else
            {
                _logger.Warn("DiscountCode {0} was a Subscription");
            }
            discount.Notes = forNotes.ToString();

        }

        private static void CalculateDiscountRedemtion(Discount discount, OrderRow row)
        {
            if (row.RegistrationType.OptionLabel.StartsWith("Live Plus Five"))
            {
                discount.CreditsRemain = discount.CreditsRemain - 1;
            }
            if (row.RegistrationType.OptionLabel == ("OnDemand Recording Only"))
            {
                discount.CreditsRemain = discount.CreditsRemain - 1.25M;
            }
            if (row.RegistrationType.OptionLabel == ("CD-ROM and Hardcopy Handouts"))
            {
                discount.CreditsRemain = discount.CreditsRemain - 1.25M;
            }
            if (row.RegistrationType.OptionLabel.StartsWith("Live Plus Six"))
            {
                discount.CreditsRemain = discount.CreditsRemain - 1.25M;
            }
            if (row.RegistrationType.OptionLabel == ("Premier Package"))
            {
                discount.CreditsRemain = discount.CreditsRemain - 1.5M;
            }
        }

        private void RejectDiscount(OrderRow orderRow)
        {
            orderRow.Discount.CreditsRemain++;
            //according to legacy code but can a condition exist 
            //  a non-valid discount resulted in a decrement.
        }

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

        public IList<Order> GetV3OrdersByWebinar(int idWebinar)
        {
            return _webinarRepository.GetOrdersByWebinar(idWebinar).ToList();
        }


        public WebUser GetWebUserWithAddressAndInstitution(int idUser)
        {
            return _webUserRepository.GetWebUserByIdLoadedWithAddressesAndInstitution(idUser);
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

                if (!ReferenceEquals(httpWebResponse, null))
                {
                    string responsePayload = ProcessErrorByStatusCode(httpWebResponse);

                    _logger.Error("Citrix Message: {0}", responsePayload);

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
