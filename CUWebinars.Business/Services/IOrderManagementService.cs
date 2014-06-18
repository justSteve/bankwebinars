using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService : IDisposable
    {
        void AddOrderRow(Order currentOrder, OrderRow orderRow);
        void AddWebinar(Webinar webinar);
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
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteWebinar(int webinarId);
        void DispatchDummyOrder();
        void FireOrderSubmittedEvent(Order order);
        IEnumerable<RegType> FindRegTypesByWebinarId(int webinarId);
        void FireSendConnectionInfoNotificationEvent(IList<Order> orders);
        void FireSendOrderShippedNotificationEvent(IList<Order> orders);
        void FireSendRecordingIsPostedEvent(IList<Order> orders);
        void FireSendReminderNotificationEvent(IList<Order> orders);
        Affiliate GetAffiliateById(int id);
        IEnumerable<Webinar> GetAllActive();
        IEnumerable<Presenter> GetAllPresenters();
        IEnumerable<Webinar> GetByTopic(int topicId);
        Discount GetDiscount(string email);
        IList<RegType> GetOptionsByWebinarId(int id, bool detached);
        Order GetOrderById(int id);
        string GetOrderInitiator();
        IList<Order> GetOrdersByUserId(int id);
        IList<Order> GetOrdersForLiveNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedNotifications(int idWebinar);
        IEnumerable<Order> GetOrdersForShippedNotification();
        OrderRow GetOrderRowById(int idOrderRow);
        IEnumerable<Webinar> GetRecordedWebinars();
        IList<RegType> GetRegTypesByWebinarIdFrom(int id, bool detached);
        IEnumerable<Topic> GetTopicsPerWebinar(int idWebinar);
        IEnumerable<Webinar> GetUpcomingWebinars();
        Webinar GetWebinar(int id);
        WebUser GetWebUser(int id);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        OrderRow LoadOrderRow(int id);
        Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        void UpdateWebinar(Webinar webinar);
    }
}
