using System.IO;
using System.Net;
using System.Text;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;
using IEventSource = CUWebinars.NotificationSystem.Event.IEventSource;

namespace CUWebinars.Business.Services
{
    public class OrderManagementService : IOrderManagementService, IEventSource
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
            //var affilateToAssign = _affiliateRepository.FindByIdAndDetachItem(affiliate.idUserAff);
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
                var regType = _regTypeRepository.FindRegType(registrationType);
                return _orderRepository.CreateOrderRow(webinar, additionalLocation, regType);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("CreateOrderRow", exception);
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
                _logger.ErrorException("CreateOrderRow", exception);
            }

            return null;
        }


        public AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname)
        {
            return _orderRepository.CreateAdditionalLocation(email, price, fullname);
        }

        public Affiliate GetAffiliateById(int id)
        {
            return _affiliateRepository.FindById(id);
        }

        public IList<RegType> GetOptionsByWebinarId(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
        }
        public IList<RegType> GetOptionsByWebinarIdFromOptionsRepository(int id, bool detached)
        {
            return _regTypeRepository.FindRegTypesByWebinarId(id, false);
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

        public void CreateOrderEvent(Order order, UserAccount userAccount)
        {
            //AddEvent(new OrderSubmittedEvent<UserAccount> { Account = userAccount, Order = order });
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

            foreach (OrderRow row in order.OrderRows)
            {
                //var option = _RegTypeRepository.FindRegType(row.RegistrationType);

                //if (option.Price != null) row.UnitPrice = (decimal)option.Price;
                //(decimal)OptionsFacade.Instance.Load((int)row.RegistrationType).Price;

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
        }

        public virtual decimal CalculateOptionsPrice(OrderRow row)
        {
            //if (row.RegistrationType == RegistrationType.SubscriptionRedeem)
            //{
            //    return 0.0M;
            //}

            decimal optionsTotal = 0.0M;
            //foreach (var option in row.AdditionalLocation)
            //{
            //    var AdditionalLocation = option.AdditionalLocation;

            //    if (AdditionalLocation == null) continue;

            //    if (row.Webinar.Title.Contains("Compliance Perspectives")//.IsSubscriptionWebinar
            //        && AdditionalLocation.Count < 4)
            //    {
            //        continue; //Subscription webinar with up to 3 additional locations. Do not charge.
            //    }

            //    if (row.Webinar.Title.Contains("Compliance Perspectives"))
            //    {

            //        int subscriptionPeriod = 12;//row.RegistrationType == RegistrationType.Twelve_Month_Subscription ? 12 : 6;
            //        optionsTotal += (AdditionalLocation.Count - 3) * option.RegTypePrice * subscriptionPeriod;
            //    }
            //    else
            //    {
            //        optionsTotal += AdditionalLocation.Count * option.RegTypePrice;
            //    }
            //}
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


        public Order SaveOrderChanges(Order currentOrder)
        {
            try
            {
                ProcessDiscountCodes(currentOrder);
                CalculateOrderPrices(currentOrder);

                var updatedOrder = _orderRepository.SaveOrderChanges(currentOrder);

                foreach (var evt in GetEvents())
                {
                    _ttsConfig.NotificationEventBus.RaiseEvent(evt);
                }

                return updatedOrder;

            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
            return null;
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

            AddEvent(new OrderSubmittedEvent<Order> { Order = order });

            return order;
        }

        public Discount GetDiscount(string email)
        {
            return null;
        }

        public string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey)
        {
            //https://www2.gotomeeting.com/en_US/island/webinar/audio/organizers/conferenceInfo.tmpl?webinarId=495203178&role=0
            //0 = attendee
            //2 = panelist
            //1 = organizer

            string orgKey = "922930"; //steve's
            //string orgKey = "901873";//marks
            string access_token = "5jxY3KZL48HWknOaOEP2eIzVmOTS"; //steve's
            //string access_token = "JIOHRkkCvmIKDY8QO0S4msbYH48N";//mark's

            //string url = "https://api.citrixonline.com/G2W/rest/organizers/" + orgKey + "/webinars/495203178/registrants";
            string url = "https://api.citrixonline.com/G2W/rest/organizers/" + orgKey + "/webinars/" + idWebinar +
                         "/registrants";


            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
            //httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Accept = "application/json";
            httpWebRequest.Accept = "application/vnd.citrix.g2wapi-v1.1+json";
            httpWebRequest.Headers.Add("Authorization", "OAuth oauth_token=" + access_token);

            httpWebRequest.Method = "POST";

            object sendVars = new {firstName = firstName, lastName = lastName, email = billingEmail};

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
                HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse();
                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                Console.WriteLine("Response stream received.");
                var myResponse = readStream.ReadToEnd();
                response.Close();
                readStream.Close();

                return myResponse;
            }
            catch (WebException webException)
            {
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
            catch (Exception ex)
            {
                _logger.ErrorException("CreateRegistrantKey", ex);
                throw;
            }
            return null;
        }

        string IOrderManagementService.CreateCalendarEvent(string title, string body, DateTime startDate, double duration, string location,
            string organizer, string eventId, bool allDayEvent)
        {
            return CreateCalendarEvent(title, body, startDate, duration, location, organizer, eventId, allDayEvent);
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
