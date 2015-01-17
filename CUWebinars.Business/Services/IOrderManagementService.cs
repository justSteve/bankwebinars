using System.Linq.Expressions;
using CUWebinars.Business.Core;
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
        //string BuildConnectionInfo(OrderRow orderRow);
        PricesAndDiscounts CalculateOrderCost(Order order, decimal optionsCost);
        int CheckUserForRecordingAccess(int i, int i1);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname);

        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteOrder(int orderId);
        Affiliate DetermineAffiliateByAlternativeMeans(int idUser);
        void DispatchDummyOrder();
        void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, Uri url = null);
        void FireSendConnectionInfoNotificationEvent(IList<Order> orders);
        void FireSendOrderShippedNotificationEvent(IList<Order> orders);
        void FireSendRecordingIsPostedEvent(IList<Order> orders);
        void FireSendReminderNotificationEvent(IList<Order> orders);
        Tuple<string, decimal> GetCostOfAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations, int idWebinar);
        Affiliate GetAffiliateByDomain(string domain);
        Affiliate GetAffiliateById(int id);
        Affiliate GetAffiliateByIdLoaded(int id, params Expression<Func<Affiliate, object>>[] includeProperties);
        IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached);
        Order GetOrderById(int id);
        IList<Order> GetOrdersByUserId(int id);
        IList<Order> GetOrdersForLiveNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedNotifications(int idWebinar);
        IEnumerable<Order> GetOrdersForShippedNotification();
        OrderRow GetOrderRowById(int idOrderRow);
        IDictionary<RegType, bool> GetRegTypesByWebinarId(int id, bool detached);
        IList<RegType> GetRegTypeOption(int optionId);
        WebUser GetWebUser(string email);
        WebUser GetWebUser(int id);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        OrderRow LoadOrderRow(int id);

        Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink, OrderGenesis orderGenesis = OrderGenesis.ImportedForExistingUser);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        string UpdateOrderChanges(Order currentOrder, ref PricesAndDiscounts pricesAndDiscounts);
        void UpdateOrderWithUserEmail(int orderId, string email);
        void UpdateOrderWithUserId(int orderId, int userId);
        void RemoveAdditionalLocationsForOrder(int idOrderRow);
        Discount GetDiscountByCode(string discount);
        decimal GetPriceOfAdditionalLocation(int idWebinar);
        void UpdateOrderByAdmin(Order order);
        Discount GetDiscountByUser(WebUser currentUser);
        void GetJoinUrl(OrderRow row);
        Discount ApplyDiscountCode(string code);
    }
}
