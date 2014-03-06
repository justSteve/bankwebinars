using System;
using System.Collections.Generic;
using System.Linq;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Events;
using CUWebinars.Business.Repository;
using CUWebinars.NotificationSystem.Event;
using IEvent = CUWebinars.NotificationSystem.Event.IEvent;
using IEventSource = CUWebinars.NotificationSystem.Event.IEventSource;

namespace CUWebinars.Business.Services
{
    public class OrderManagementService : IOrderManagementService, IEventSource
    {
        private readonly IAffiliateRepository _affiliateRepository;
        private readonly IOptionRepository _optionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRefDataRepository _refDataRepository;
        private readonly IWebUserRepository _webUserRepository;
        private readonly TtsConfiguration _ttsConfig;
        List<IEvent> events = new List<IEvent>();

        public OrderManagementService(
            IAffiliateRepository affiliateRepository, 
            IOptionRepository optionRepository, 
            IOrderRepository orderRepository, 
            IRefDataRepository refDataRepository, 
            IWebUserRepository webUserRepository,
            TtsConfiguration ttsConfig)
        {
            _affiliateRepository = affiliateRepository;
            _optionRepository = optionRepository;
            _orderRepository = orderRepository;
            _refDataRepository = refDataRepository;
            _ttsConfig = ttsConfig;
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

            var option = _optionRepository.FindOption((int)orderRow.RegistrationType);

            return string.Empty;
        }

        public Order CreateNewOrder()
        {
            return _orderRepository.CreateOrder();
        }

        public IList<Option> GetOptionsByWebinarId(int id)
        {
            return _refDataRepository.FindOptionsByWebinarId(id);
        }

        public IList<Order> GetOrdersByUserId(int id)
        {
            var sendback = _refDataRepository.FindOrdersByUserId(id);

            return sendback;
        }

        public void CreateOrderEvent(Order order, UserAccount userAccount)
        {
            AddEvent(new OrderSubmittedEvent<UserAccount>{ Account = userAccount, Order = order });
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
        //public static WebUser GetMasterUser(this UserFacade userFacadeInstance)
        //{
        //    if (userFacadeInstance.IsCurrentUserControlled() == false)
        //    {
        //        throw new TTSException("There is no controlled user");
        //    }

        //    return GetAuthenticatedUser(userFacadeInstance);
        //}

        public byte GetOrderInitiator()
        {
            return 0;
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return events;
        }

        protected void AddEvent<TE>(TE orderEvent) where TE : IEvent
        {
            if (orderEvent is IAllowMultiple || events.All(x => x.GetType() != orderEvent.GetType()))
            {
                events.Add(orderEvent);
            }
        }


        private void CalculateOrderPrices(Order order)
        {
            order.Total = 0.0M;

            //foreach (OrderRow row in order.Rows)
            //{

            //    row.UnitPrice = (decimal)OptionsFacade.Instance.Load((int)row.RegistrationType).PriceToAdd;

            //    //Calculate options price
            //    decimal optionsTotal = CalculateOptionsPrice(row);

            //    //Calculate row price before discount
            //    row.RowPrice = row.UnitPrice + optionsTotal;

            //    //Calculate discount. Discount is not valid for subscription webinars.
            //    if (row.Webinar.IsSubscriptionWebinar == false)
            //    {
            //        decimal discountTotal = row.DiscountFlatOff;
            //        if (row.DiscountPercentOff != 0.0M)
            //        {
            //            discountTotal = row.RowPrice * row.DiscountPercentOff / 100;
            //        }
            //        if (discountTotal > row.RowPrice)
            //        {
            //            discountTotal = row.RowPrice;
            //        }
            //        row.RowPrice = row.RowPrice - discountTotal;
            //    }

            //    //Calculate order total
            //    order.Total += row.RowPrice;
            //}
        }

        public virtual decimal CalculateOptionsPrice(OrderRow row)
        {
            //if (row.RegistrationType == RegistrationType.SubscriptionRedeem)
            //{
            //    return 0.0M;
            //}

            decimal optionsTotal = 0.0M;
            foreach (OrderRowOption option in row.OrderRowOptions
                //.Options
                )
            {
                var locations = option as AdditionalLocationsOrderRowOption;
                if (locations != null)
                {
                    AdditionalLocationsOrderRowOption additionalLocations = locations;
                    if (row.Webinar.Title.Contains("Compliance Perspectives")
                        //.IsSubscriptionWebinar
                        && additionalLocations.AdditionalLocationsCount < 4)
                    {
                        continue; //Subscription webinar with up to 3 additional locations. Do not charge.
                    }

                    if (row.Webinar.Title.Contains("Compliance Perspectives"))
                    {

                        int subscriptionPeriod = 12;//row.RegistrationType == RegistrationType.Twelve_Month_Subscription ? 12 : 6;
                        optionsTotal += (additionalLocations.AdditionalLocationsCount - 3) * additionalLocations.OptionPrice * subscriptionPeriod;
                    }
                    else
                    {
                        optionsTotal += additionalLocations.AdditionalLocationsCount * additionalLocations.OptionPrice;
                    }
                }
            }
            return optionsTotal;
        }
        public virtual void AssignUserToOrder(Order order, WebUser user)
        {
            order.WebUser = user;
            //order.Affiliate = user.Affiliate;

            order.BillingAddress = user.Addresses.Where(a => a.AddressType == "Billing").Select(a => a.StreetAddress).ToString();
            order.BillingCity ="City";
            order.BillingEmail = "user.Email";
            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.BillingPhone = "user.Phone1";
            order.ShippingAddress = "user.ShippingAddress";

            order.ShippingCity = "user.ShippingCity";
            order.ShippingFirstName = "user.ShippingFirstName";
            order.ShippingLastName = "user.ShippingLastName";
            order.ShippingPhone = "user.ShippingPhone";
            order.ShippingState = "user.ShippingState";
            order.ShippingZip = "user.ShippingZip";
            order.ShippingState = "user.State";
            order.BillingState = "user.State";
            order.ShippingZip = "user.Zip";
            order.BillingZip = "user.Zip";
            //order.Affiliate = user.Affiliate;
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

        public void CreateCPSubscription(OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public void Save(Order currentOrder)
        {
            ProcessDiscountCodes(currentOrder);
            CalculateOrderPrices(currentOrder);

            //base.Save(instance);
            throw new NotImplementedException();
        }

        public Order SaveChanges(Order currentOrder)
        {
            return _orderRepository.SaveOrder(currentOrder);
        }

        private void ProcessDiscountCodes(object instance)
        {
            throw new NotImplementedException();
        }

        public void AddOrderRow(Order currentOrder, OrderRow orderRow)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            events.Clear();
        }
    }
}
