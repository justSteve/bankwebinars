using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Exceptions;
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
        private readonly ILogger _logger;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        readonly List<IEvent> _events = new List<IEvent>();

        public OrderManagementService(
            IAffiliateRepository affiliateRepository,
            IRegTypeRepository regTypeRepository,
            IOrderRepository orderRepository,
            IRefDataRepository refDataRepository,
            IWebUserRepository webUserRepository,
            IWebinarRepository webinarRepository,
            ILogger logger,
            TtsConfiguration ttsConfig)
        {
            _affiliateRepository = affiliateRepository;
            _regTypeRepository = regTypeRepository;
            _orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
            _webinarRepository = webinarRepository;
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
                throw new Exception("Booya");
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
            if(string.IsNullOrWhiteSpace(email))
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

        public Affiliate GetAffiliateById(int id)
        {
            return _affiliateRepository.FindById(id);
        }

        public IEnumerable<Webinar> GetAllActive()
        {
            try
            {
                return _webinarRepository.GetAllActive().ToList();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllActive method", exception);
                throw;
            }
        }

        public IEnumerable<Presenter> GetAllPresenters()
        {
            try
            {
                return _refDataRepository.GetAllPresenters();
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetAllPresenters method", exception);
                throw;
            }

        }

        public IEnumerable<Webinar> GetByTopic(int topicId)
        {
            return _webinarRepository.GetByTopic(topicId);
        }


        public IList<RegType> GetOptionsByWebinarId(int id, bool detached)
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

        public IList<RegType> GetRegTypesByWebinarIdFrom(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
            return null;
        }

        public IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return _webinarRepository.GetTopicsPerWebinar(idWebinar).ToList();
        }

        public OrderRow GetOrderRowById(int idOrderRow)
        {
            return _orderRepository.GetOrderRowById(idOrderRow);
        }

        public IEnumerable<Webinar> GetRecordedWebinars()
        {
            return _webinarRepository.GetRecorded().ToList();
        }
        public IEnumerable<Webinar> GetUpcomingWebinars()
        {
            return _webinarRepository.GetUpcoming().ToList();
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

        public Webinar GetWebinar(int id)
        {
            return _webinarRepository.FindByIdLoaded(id);
        }
        public WebUser GetWebUser(int id)
        {
            return _webUserRepository.FindByIdLoaded(id);
        }

        public IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar)
        {
            return _webUserRepository.GetWebusersForLiveNotifications(idWebinar);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            try
            {
                var webinar = _webinarRepository.GetWebinarByIdIncludingAllWebinarsByPresenter(id);
                return webinar;
            }
            catch (Exception exception)
            {
                _logger.ErrorException("GetWebinarByIdIncludingAllWebinarsByPresenter", exception);
                throw;
            }

        }


        public void AddWebinar(Webinar webinar)
        {
            _webinarRepository.Add(webinar);
        }

        public void DeleteWebinar(int webinarId)
        {
            var webinar = GetWebinar(webinarId);
            _webinarRepository.Delete(webinar);
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


        private void CalculateOrderPrices(Order order)
        {
            order.Total = 0.0M;

            var row = order.OrderRows.Single();

            if (row.RegistrationType.Price != null) row.UnitPrice = (decimal)row.RegistrationType.Price;
            //
            //Calculate options price
            decimal optionsTotal = CalculateOptionsPrice(row);

            //Calculate row price before discount
            row.RowPrice = row.UnitPrice + optionsTotal;

            //Calculate discount. Discount is not valid for subscription webinars.
            if (row.Webinar.Title.Contains("Compliance Perspectives") == false)
            {

                //decimal discountTotal = row.
                //if (row.DiscountPercentOff != 0.0M)
                //{
                //    discountTotal = row.RowPrice * row.DiscountPercentOff / 100;
                //}
                //if (discountTotal > row.RowPrice)
                //{
                //    discountTotal = row.RowPrice;
                //}
                //row.RowPrice = row.RowPrice - discountTotal;
            }

            //Calculate order total
            order.Total += row.RowPrice;

        }

        public virtual decimal CalculateOptionsPrice(OrderRow row)
        {
            //if (row.RegistrationType == RegistrationType.SubscriptionRedeem)
            //{
            //    return 0.0M;
            //}

            decimal optionsTotal = 0.0M;
            foreach (var option in row.AdditionalLocation)
            {
                //var AdditionalLocation = option.AdditionalLocation;

                //if (AdditionalLocation == null) continue;

                //if (row.Webinar.Title.Contains("Compliance Perspectives")//.IsSubscriptionWebinar
                //    && AdditionalLocation.Count < 4)
                //{
                //    continue; //Subscription webinar with up to 3 additional locations. Do not charge.
                //}

                //if (row.Webinar.Title.Contains("Compliance Perspectives"))
                //{

                //    int subscriptionPeriod = 12;//row.RegistrationType == RegistrationType.Twelve_Month_Subscription ? 12 : 6;
                //    optionsTotal += (AdditionalLocation.Count - 3) * option.RegTypePrice * subscriptionPeriod;
                //}
                //else
                //{
                //    optionsTotal += AdditionalLocation.Count * option.RegTypePrice;
                //}
                var a = 1;
            }
            return optionsTotal;
        }
        public virtual void AssignUserToOrder(Order order)
        {
            var user = order.WebUser;

            var billingAddress = user.Addresses.FirstOrDefault(a => a.AddressType == "Billing");
            var shippingAddress = user.Addresses.FirstOrDefault(a => a.AddressType == "Shipping");

            if (billingAddress != null)
            {

                order.BillingAddress = billingAddress.StreetAddress;
                order.BillingCity = billingAddress.City;
                order.BillingEmail = user.email;
                order.FirstName = user.FirstName;
                order.LastName = user.LastName;
                order.BillingPhone = billingAddress.Phone;
                order.BillingZip = billingAddress.Zip;
                order.BillingState = billingAddress.State;

                if (shippingAddress != null)
                {
                    order.ShippingAddress = shippingAddress.StreetAddress;
                    order.ShippingCity = shippingAddress.City;
                    order.ShippingFirstName = user.FirstName;
                    order.ShippingLastName = user.LastName;
                    order.ShippingPhone = shippingAddress.Phone;
                    order.ShippingState = shippingAddress.State;
                    order.ShippingZip = shippingAddress.Zip;
                    order.Institution = user.Institution.InstitutionName;

                    //if (user.SubscriptionDiscount != null)
                    //{
                    //    foreach (OrderRow row in order.Rows)
                    //    {
                    //        row.DiscountCode = user.SubscriptionDiscount.Code;
                    //    }
                    //}
                    //try
                    //{
                    //    Save(order);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Logger.Instance.LogException(ex);
                    //    Logger.Instance.LogMessage("ERROR: Failed to save user assigned to order: "+ user.Email + " order = " + order.ID);
                    //    throw;
                    //}
                }
            }
        }

        public void CreateCPSubscription(OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RegType> FindRegTypesByWebinarId(int webinarId)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(webinarId, false);
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

            var relativePath = Path.Combine(@"App_Data\Notifications", string.Format("OrderNotification-{0}{1}", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff-tt"), ".htm"));

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

        public Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink)
        {
            try
            {
                ProcessDiscountCodes(currentOrder);
                CalculateOrderPrices(currentOrder);

                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder);

                bool linkToVerifyAccount = !string.IsNullOrWhiteSpace(confirmChangeEmailLink);

                _logger.Info("Adding Event for Order {0}", currentOrder.idOrder);

                
                var orderSubmittedViewModel = new OrderSubmittedViewModel
                {
                    ConfirmChangeEmailUrl = linkToVerifyAccount ? string.Concat(confirmChangeEmailLink.Replace(DomainConstants.Blank, string.Empty), currentOrder.WebUser.LastName.ToLower()) : string.Empty,
                    Order = updatedOrder,
                    UserCreatedOnImport = linkToVerifyAccount
                };

                var relativePath = Path.Combine(@"App_Data\Notifications", string.Format("OrderNotification-{0}{1}", DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff-tt"), ".htm"));

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
            return  _orderRepository.SelectOrdersWithScheduledWebinars(idUser);
        }

        public void UpdateWebinar(Webinar webinar)
        {
            _webinarRepository.Update(webinar);
        }

        public int CheckUserForRecordingAccess(int w, int u)
        {
            return _orderRepository.CheckUserForRecordingAccess(w, u);
        }
        

        private void ProcessDiscountCodes(object instance)
        {
            var a = 1;
        }

        public void AddOrderRow(Order currentOrder, OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {

            var order = _orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow);
            _logger.Info("CreateNewOrder: " + webUser.email + "| " + orderRow.Webinar.Title + "| " + orderRow.RegistrationType.OptionLabel);
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

                // Pipes the stream to a higher level stream reader with the required encoding format. 
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
                _logger.ErrorException(string.Format("CreateRegistrantKey WebException: {0}, {1}. ExceptionMsg = {2}",billingEmail, webinarKey, webException.Message), webException);
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

        string IOrderManagementService.CreateCalendarEvent(string title, 
            string body, 
            DateTime startDate, 
            double duration, 
            string location,
            string organizer, 
            string eventId, 
            bool allDayEvent)
        {
            try
            {
                return CreateCalendarEvent(title, body, startDate, duration, location, organizer, eventId, allDayEvent);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Title:{0},body:{1},startDate:{2},duration{3},location{4},organizer{5},eventId{6},allDayEvent" +
                                                     "{7}", title, body, startDate.ToString("yyyy-MM-dd-hh-mm-ss-fff-tt"), duration, location, 
                                                     organizer, eventId, allDayEvent), 
                                                     exception
                                                     );
                throw;
            }
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
    }
}
