using System.Linq.Expressions;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.ViewModel;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService : IDisposable
    {
        void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation);
        Order AssignAffiliateToOrder(int affiliateId, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        Affiliate AttachAffiliate(Affiliate item);
        //string BuildConnectionInfo(OrderRow orderRow);
        PricesAndDiscounts CalculateOrderCost(Order order, decimal optionsCost);
        int CheckUserForRecordingAccess(int i, int i1);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullname);

        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        Order CreateNewOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);

        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteOrder(int orderId);
        Affiliate DetermineAffiliateByAlternativeMeans(int idUser);
        void DispatchDummyOrder();
        IEnumerable<Order> FindOrdersByUserId(int userId);
        void FireAdhocNotificationHandler(AdhocNotificationMessage adhocNotificationMessage);
        void FireAdminEmailConnectionInfoHandler(Order order, IEnumerable<string> recipients);
        void FireAdminEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients, bool resending = false);
        void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false, Uri url = null);
        void FireOrderSynchEvent(Order order, bool userCreatedInCart = false, bool resending = false, Uri url = null);
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
        IDictionary<RegType, bool> GetAllPossibleOptionsByWebinarId(int idWebinar, bool detached);
        IEnumerable<Order> GetOrdersByEmail(string email, int aff);
        Order FindExpressCheckoutOrder(string email, int idWebinar);
        IEnumerable<Order> GetOrdersByLastName(string lastName, int aff);
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
        Discount GetDiscountByUser(WebUser currentUser);
        void GetJoinUrl(OrderRow row);
        Discount ApplyDiscountCode(string code, OrderRow row);
        void GenerateRegistrantKey(Order order, AdditionalLocation additionalLocation = null);
        int GetNumberOfOrdersPerWebinar(int id);
        void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation);
        
        Discount GetDiscountById(int discount);
        IList<Order> GetV3OrdersByWebinar(int idWebinar);
        DateTime CalculatePostEventMaterialsAccessExpiry(Order order);
        //void SendOrderToLegacy(Order newOrder);
        WebUser GetWebUserWithAddressAndInstitution(int idUser);
        IEnumerable<int> GetUserIdsByPartialId(int value);
        object SearchRegistrations(int affiliateID, IList<int> excludeUserIDs, int skip, int take, string search);
        
        string SetPostEventClaims(int webinarId);

        bool VerifyWebUserExists(int idUser);
        Webinar GetWebinarByJoinCode(string joinCode);
        void LoadWebinarIntoOrderRow(OrderRow newOrderRow);
        void SetUserStatusToUnChanged(WebUser user);
        void SetAffiliateStatusToUnChanged(Affiliate affiliate);
        IEnumerable<Order> GetOrdersByEmailDomain(string email, int aff);
        void SendAdhocNotification(string emails, string subject, string body);
        OrderRow GetLegacyOrder(Order order);
        void SynchOrders(int webinarId);
        Order FindExpressCheckoutOrderByOrderId(int q11Orderid);
        string GetAccessToRecording(Order order);
        IEnumerable<Order> GetV3OrdersByWebinarForPostEventClaims(int idWebinar);
        void UpdateOrderByAdmin(Order newOrder);
        int SynchExpressCheckoutOrder(Order order);
        void SynchIds(Order order);
        PostEventClaim FindPostEventClaimByOnDemandCode(Order order);
        IList<Order> GetV3OrdersByOnDemandClaim();
        IList<PostEventClaim> FindAllPostEventClaims();
        Webinar GetWebinarById(int webinarId);
    }
}
