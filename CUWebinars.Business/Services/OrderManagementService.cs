using System.Configuration;
using System.Diagnostics;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Migrations;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using Newtonsoft.Json;
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
        }

        public Order AssignAffiliateToOrder(Affiliate affiliate, Order order)
        {
            return _orderRepository.AssignAffiliate(affiliate, order);
        }

        public Order AssignWebUserToOrder(WebUser webUser, Order order)
        {
            return _orderRepository.AssignWebUserToOrder(webUser, order);
        }

        public Affiliate AttachAffiliate(Affiliate item)
        {
            item.Orders = null;
            return _affiliateRepository.Exists(item) ? item : _affiliateRepository.AttachItem(item);
        }

        public string BuildConnectionInfo(OrderRow orderRow)
        {
            // add code here to build string

            //var option = _RegTypeRepository.FindRegType(orderRow.RegistrationType);

            return string.Empty;
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
                _logger.ErrorException("CreateOrderRow method", exception);
                //todo: can elmah be persuaded to fire from thic class? 
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
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

        public Tuple<string, decimal> GetAdditionalLocationsPricing(IEnumerable<AdditionalLocation> additionalLocations, int idWebinar)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var addresses = new StringBuilder();
            decimal optionsCost = 0M;
            var i = 0;

            var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(idWebinar);

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

                Debug.Assert(additionalLocationsPricing.Count == 1,
                    "There should only ever be 1 value returned for the cost of an Additionalocation for a particular Webinar"
                    );
                // Item2 of the tuple is the price value as a decimal. Item 1 is the AdditionalLocationsLookupPrice id 
                optionsCost += additionalLocationsPricing.Single().Item2;
            }

            return new Tuple<string, decimal>(addresses.ToString(), optionsCost);
        }

        public Affiliate GetAffiliateById(int id)
        {
            return _affiliateRepository.FindById(id);
        }



        public IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
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

        public IDictionary<RegType, bool> GetRegTypesByWebinarIdFrom(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
        }

        public IList<RegType> GetRegTypesForOption(int optionId)
        {
            return _regTypeRepository.FindRegTypesForOption(optionId);
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
                _logger.ErrorException("GetOrdersByUserId", exception);
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
                throw new EntityNotFoundException(ex.Message, ex);
            }
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

        public string GetOrderInitiator()
        {
            //TODO: GetOrderInitiator() returns the source of the order (migrated, imported, end-user, affiliate)
            return "0";
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
        public PricesAndDiscounts CalculateOrderPrices(Order order, decimal optionsCost)
        {
            order.Total = 0.0M;
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);

            var row = order.OrderRows.FirstOrDefault();

            foreach (var orderRow in order.OrderRows)
            {
                if (orderRow.RowStatus == OrderRowStatus.Active)
                {
                    row = orderRow;
                }
            }

            if (row != null) row.UnitPrice = (decimal)row.RegistrationType.Price;

            //Calculate row price before discount
            row.RowPrice = row.UnitPrice + optionsCost;
            pricesAndDiscounts.UnitPrice = row.UnitPrice;

            //Calculate discount. 
            decimal discountTotal = 0;

            if (row.Discount != null && row.Discount.percentOff != 0.0M)
            {
                discountTotal = row.RowPrice * row.Discount.percentOff / 100;
            }
            else if (row.Discount != null && row.Discount.flatOff != 0.0M)
            {
                discountTotal = row.Discount.flatOff;
            }

            if (discountTotal > row.RowPrice)
            {
                discountTotal = row.RowPrice;
            }

            row.RowPrice -= discountTotal;
            pricesAndDiscounts.TotalDiscount = discountTotal;
            pricesAndDiscounts.TotalOptions = optionsCost;

            //Calculate order total
            order.Total += row.RowPrice;
            pricesAndDiscounts.TotalOrderPrice = order.Total;

            return pricesAndDiscounts;
        }

        //public virtual decimal CalculateOptionsPrice(OrderRow row)
        //{
        //    ////if (row.RegistrationType == RegistrationType.SubscriptionRedeem)
        //    ////{
        //    ////    return 0.0M;
        //    ////}

        //    //var additionalLocationsPricingForWebinar = GetAdditionalLocationsPricing(row.idWebinar);

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

        public void FireOrderSubmittedEvent(Order order)
        {
            _logger.Info("Adding Event for Order {0}", order.idOrder);

            var orderSubmittedViewModel = new OrderSubmittedViewModel
            {
                ConfirmChangeEmailUrl = string.Empty,
                Order = order,
                UserCreatedOnImport = false
            };

            var relativePath = Path.Combine(@"App_Data\Notifications", string.Format("OrderNotification-{0}{1}", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm"));

            AddEvent(new OrderSubmittedEvent<OrderSubmittedViewModel>
            {
                EventObject = orderSubmittedViewModel,
                RelativeFilePath = relativePath
            });


            _logger.Info("Persisted Email for Order {0}:{1}", order.idOrder, relativePath);


            foreach (var evt in GetEvents())
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

            foreach (var evt in GetEvents())
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

            foreach (var evt in GetEvents())
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


            foreach (var evt in GetEvents())
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


            foreach (var evt in GetEvents())
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


            foreach (var evt in GetEvents())
            {
                _ttsConfig.NotificationEventBus.RaiseEvent(evt);
            }

            Clear();
        }

        public string UpdateOrderChanges(Order currentOrder)
        {
            try
            {
                var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
                var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(currentOrder.OrderRows.Single().idWebinar);
                ProcessDiscountCodes(currentOrder);
                CalculateOrderPrices(currentOrder, additionalLocationsPricing.Single().Item2);

                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, 0);


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
            order.AuditInfo = "Placeholder user is replaced by " + user.email + Environment.NewLine + order.AuditInfo;
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
            _additionalLocationsRepository.DeleteAdditionalLocationsByOrderRowId(idOrderRow);
        }

        public Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);
            var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(currentOrder.OrderRows.Single().idWebinar);

            ProcessDiscountCodes(currentOrder);
            CalculateOrderPrices(currentOrder, additionalLocationsPricing.Single().Item2);

            if (confirmChangeEmailLink == string.Empty)
            {
                //we are saving a non-confirmed order
                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, 1);

                return updatedOrder;

            }

            try
            {
                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder, 0);

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
                    UserCreatedOnImport = linkToVerifyAccount
                };

                var relativePath = Path.Combine(@"App_Data\Notifications",
                    string.Format("OrderNotification-{0}{1}",
                        DateTime.Now.ToString(DomainConstants.DateTimeLongFormat), ".htm"));

                AddEvent(new OrderSubmittedEvent<OrderSubmittedViewModel>
                {
                    EventObject = orderSubmittedViewModel,
                    RelativeFilePath = relativePath
                });

                _logger.Info("Persisted Email for Order {0}:{1}", currentOrder.idOrder, relativePath);

                foreach (var evt in GetEvents())
                {
                    _ttsConfig.NotificationEventBus.RaiseEvent(evt);
                }
                //what's the Clear() do?
                // [dar] it clears the EventBus. From recollection, this was necessary in the "batch create order" scenario
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
            //TODO Decide: Should Discounts have a Service dedicated to them?
            var row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            //legacy's verbetum if (row.Discount == null && String.IsNullOrEmpty(row.DiscountCode) == false)

            if (row.Discount != null)
            {
                RedeemDiscount(row);
                _logger.Info("Redeemed Discount On Order: " + order.idOrder);
            }
            else
            {
                var aHolder = "";
                //_logger.Error("ERROR: Rejected Discount On Order: " + order.idOrder);
            }
            //legacy: tracks error condition 
            //else if (row.Discount != null && row.Discount.Code != row.DiscountCode)
            //{
            //    RejectDiscount(row);
            //    _logger.Error("ERROR: Rejected Discount On Order: " + order.ID);

            //    if (String.IsNullOrEmpty(row.DiscountCode) == false)
            //    {
            //        RedeemDiscount(row);
            //    }
            //}

        }
        public virtual bool IsDiscountCodeValid(string discountCode)
        {

            return true;
        }
        
        
        private void RedeemDiscount(OrderRow orderRow)
        {
            Discount discount = orderRow.Discount;

            if (discount.discountType != DiscountType.Subscription)
                discount.usesNumber--;
            _logger.Info("Discount was redeemed for {0}.", orderRow.idOrder);
            //TODO: Determine if OrderRow should be saved here or depend on other code in the workflow.
        }

        private void RejectDiscount(OrderRow orderRow)
        {
            orderRow.Discount.usesNumber++;
            //according to legacy code but can a condition exist 
            //  a non-valid discount resulted in a decrement.
        }

        public void AddOrderRow(Order currentOrder, OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {

            var order = _orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow);
            var email = webUser == null ? "notauthenticated@cuwebinars.com" : webUser.email;
            _logger.Info("CreateNewOrder: " + email + "| " + orderRow.Webinar.Title + "| " + orderRow.RegistrationType.OptionLabel);
            return order;
        }

        public Discount GetDiscount(string email)
        {
            return null;
        }

        public string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int webinarId,
            string webinarKey)
        {
            //https://www2.gotomeeting.com/en_US/island/webinar/audio/organizers/conferenceInfo.tmpl?webinarId=495203178&role=0
            //0 = attendee
            //2 = panelist
            //1 = organizer
            var webinar = _webinarRepository.FindById(webinarId);
            string orgKey = webinar.OrganizerKey;
            //string orgKey = "922930"; //steve's
            ////string orgKey = "901873";//marks
            string access_token = webinar.OrganizerOAuthKey;
            //string access_token = "5jxY3KZL48HWknOaOEP2eIzVmOTS"; //steve's
            ////string access_token = "JIOHRkkCvmIKDY8QO0S4msbYH48N";//mark's


            string url = "https://api.citrixonline.com/G2W/rest/organizers/" + orgKey + "/webinars/" + webinarKey + "/registrants";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Accept = "application/vnd.citrix.g2wapi-v1.1+json";
            httpWebRequest.Headers.Add("Authorization", "OAuth oauth_token=" + access_token);

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

            try
            {
                _logger.Info("CreateRegistrantKey starts: " + billingEmail + ", " + webinarKey);
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
                _logger.ErrorException(string.Format("CreateRegistrantKey WebException: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, webException.Message), webException);
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
                _logger.ErrorException(string.Format("CreateRegistrantKey Exception: {0}, {1}. ExceptionMsg = {2}", billingEmail, webinarKey, exception.Message), exception);

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
