using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        void AddOrderRow(Order currentOrder, OrderRow orderRow);
        Order AssignAffiliateToOrder(Affiliate affiliate, Order order);
        void AssignUserToOrder(Order currentOrder);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        string BuildConnectionInfo(OrderRow orderRow);
        int CheckUserForRecordingAccess(int i, int i1);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname);
        string CreateCalendarEvent(string title, string body, DateTime startDate, double duration, string location,
            string organizer, string eventId, bool allDayEvent);
        void CreateCPSubscription(OrderRow orderRow);
        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        void CreateOrderEvent(Order order, UserAccount userAccount);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);
        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar, string webinarKey);
        void DispatchDummyOrder();
        IEnumerable<RegType> FindRegTypesByWebinarId(int webinarId);
        void FireSendConnectionInfoNotificationEvent(IList<Order> orders);
        void FireSendOrderShippedNotificationEvent(IList<Order> orders);
        void FireSendRecordingIsPostedEvent(IList<Order> orders);
        void FireSendReminderNotificationEvent(IList<Order> orders);
        Affiliate GetAffiliateById(int id);
        Discount GetDiscount(string email);
        IList<RegType> GetOptionsByWebinarId(int id, bool detached);
        Order GetOrderById(int id);
        string GetOrderInitiator();
        IList<Order> GetOrdersByUserId(int id);
        IList<Order> GetOrdersForLiveNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedNotifications(int idWebinar);
        IEnumerable<Order> GetOrdersForShippedNotification();
        IEnumerable<Webinar> GetRecordedWebinars();
        IList<RegType> GetRegTypesByWebinarIdFrom(int id, bool detached);
        IEnumerable<Webinar> GetUpcomingWebinars();
        Webinar GetWebinar(int id);
        WebUser GetWebUser(int id);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        OrderRow LoadOrderRow(int id);
        Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink);
    }
}
