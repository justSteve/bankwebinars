using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService : IDisposable
    {
        Order AssignAffiliateToOrder(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        Affiliate AttachAffiliate(Affiliate item);
        string BuildConnectionInfo(OrderRow orderRow);
        int CheckUserForRecordingAccess(int i, int i1);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname);

        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteOrder(int orderId);
        void DispatchDummyOrder();
        void FireOrderSubmittedEvent(Order order);
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
        OrderRow GetOrderRowById(int idOrderRow);
        IList<RegType> GetRegTypesByWebinarIdFrom(int id, bool detached);
        IList<RegType> GetRegTypesForOption(int optionId);
        WebUser GetWebUser(int id);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        OrderRow LoadOrderRow(int id);
        Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        string UpdateOrderChanges(Order order);
        void UpdateOrderWithUserEmail(int orderId, string email);
        void UpdateOrderWithUserId(int orderId, int userId);
        void RemoveAdditionalLocationsForOrder(int idOrderRow);
    }
}
