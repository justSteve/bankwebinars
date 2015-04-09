using System.Linq.Expressions;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using CUWebinars.Business.Notification.ViewModel;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService : IDisposable
    {
        void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation);
        Order AssignAffiliateToOrder(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        Affiliate AttachAffiliate(Affiliate item);
        //string BuildConnectionInfo(OrderRow orderRow);
        PricesAndDiscounts CalculateOrderCost(Order order, decimal optionsCost);
        int CheckUserForRecordingAccess(int i, int i1);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname);

        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow,
            string origin = null);

        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteOrder(int orderId);
        Affiliate DetermineAffiliateByAlternativeMeans(int idUser);
        void DispatchDummyOrder();
        void FireAdminEmailConnectionInfoHandler(Order order, IEnumerable<string> recipients);
        void FireAdminEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients, bool resending = false);
        void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false, Uri url = null);
        void FireSendConnectionInfoNotificationEvent(IList<Order> orders, bool resending);
        void FireSendOrderShippedNotificationEvent(IList<Order> orders);
        void FireSendRecordingIsPostedEvent(IList<Order> orders);
        void FireSendPerDayPromoEvent(WebinarPromoViewModel webinarPromoViewModel);
        void FireSendPerWeekPromoEvent(IList<Affiliate> affiliates, Webinar webinar);
        void FireSendReminderNotificationEvent(IList<Order> orders);
        IEnumerable<AdditionalLocation> GetAdditionalLocationsForOrderRow(int idOrderRow);
        IDictionary<int, string> GetAffiliatesForDisplayList();
        Tuple<string, decimal> GetCostOfAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations,
            int idWebinar);

        Affiliate GetAffiliateByDomain(string domain);
        Affiliate GetAffiliateById(int id);
        Affiliate GetAffiliateByIdLoaded(int id, params Expression<Func<Affiliate, object>>[] includeProperties);
        IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached);
        IEnumerable<Order> GetOrdersByEmail(string email);
        IEnumerable<Order> GetOrdersByLastName(string lastName);
        Order GetOrderById(int id);
        Order GetOrderByIdThin(int id);
        IEnumerable<int> GetOrderIdsByPartialId(int id);
        IList<Order> GetOrdersByUserId(int id);
        IList<Order> GetOrdersForLiveNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedNotifications(int idWebinar);
        IEnumerable<Order> GetOrdersForShippedNotification();
        OrderRow GetOrderRowById(int idOrderRow);
        IDictionary<RegType, bool> GetRegTypesByWebinarId(int id, bool detached);
        RegType GetRegTypeOfOrderRow(int idRegType);
        IList<RegType> GetRegTypeOption(int optionId);
        WebUser GetWebUser(string email);
        WebUser GetWebUser(int id);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        string GetWebUserFullname(string email);
        OrderRow LoadOrderRow(int id);
        int SaveChanges();

        Order SaveOrderChanges(Order currentOrder, string verificationKey, string confirmChangeEmailLink,
            OrderGenesis orderGenesis = OrderGenesis.ImportedForExistingUser);

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
        Discount ApplyDiscountCode(string code, OrderRow row);
        void GenerateRegistrantKey(Order order, AdditionalLocation nuller);
        int GetNumberOfOrdersPerWebinar(int id);
        void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation);
        
        Discount GetDiscountById(int discount);
        IList<Order> GetOrdersForWebinar(int idWebinar);
        string GetPostEventMaterialsAccessExpiry(Order order);
        void SendOrderToLegacy(Order newOrder);
        WebUser GetWebUserWithAddressAndInstitution(int idUser);
        IEnumerable<int> GetUserIdsByPartialId(int value);
        object SearchRegistrations(int affiliateID, IList<int> excludeUserIDs, int skip, int take, string search);
        
        string SetPostEventClaims(int webinarId);
        
    }
}
