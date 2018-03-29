using System.Linq.Expressions;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using LogMeIn.GoToWebinar.Api.Model;
using CUWebinars.Business.Notification.Email;
using CUWebinars.Business.Notification.ViewModel;
using Webinar = CUWebinars.Business.Models.Webinar;

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
        //Order CreateNewOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);

        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        Registrant CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar,
            string webinarKey);

        void DeleteOrder(int orderId);
        Affiliate DetermineAffiliateByAlternativeMeans(int idUser, int sessionAff);
        void DispatchDummyOrder();
        IEnumerable<Order> FindOrdersByUserId(int userId);
        void FireAdhocNotificationHandler(AdhocNotificationMessage adhocNotificationMessage);
        void FireAdminEmailConnectionInfoHandler(Order order, IEnumerable<string> recipients);
        void FireAdminEmailSendShippedOrderEvent(Order order, IEnumerable<string> recipients, bool resending = false);
        //void FireOrderSubmittedEvent(Order order, bool userCreatedInCart = false, bool resending = false, Uri url = null);
        void FireOrderSubmittedMultiEvent(string toEmail, string subject, string body);
        void FireOrderSubmitted2Event(string toEmail, string subject, string body);

        void FireSendConnectionInfoNotificationEvent(IList<Order> orders, bool resending);
        void FireSendOrderShippedNotificationEvent(IList<Order> orders);
        //void FireSendRecordingIsPostedEvent(IList<Order> orders);

        void FireMandrillNotificationEvent(string toEmail, string subject, string body);
        void FireSendPerDayPromoEvent(WebinarPromoViewModel webinarPromoViewModel);
        void FireSendPerWeekPromoEvent(IList<Affiliate> affiliates, Webinar webinar);
        void FireSendReminderNotificationEvent(IList<Order> orders);
        void FireSendWeeklyInvoiceEvent(SendWeeklyInvoiceViewModel weeklyInvoiceViewModel);

        IEnumerable<AdditionalLocation> GetAdditionalLocationsForOrderRow(int idOrderRow);
        IDictionary<int, string> GetAffiliatesForDisplayList();
        //Tuple<string, decimal> GetCostOfAdditionalLocations(IEnumerable<AdditionalLocation> additionalLocations,
        //    int idWebinar);

        Affiliate GetAffiliateByDomain(string domain);
        Affiliate GetAffiliateById(int id);
        Affiliate GetAffiliateByIdLoaded(int id, params Expression<Func<Affiliate, object>>[] includeProperties);
        IDictionary<RegType, bool> GetOptionsByWebinarId(int id, bool detached);
        IDictionary<RegType, bool> GetAllPossibleRegTypesByWebinarId(int idWebinar, bool detached);
        IList<RegType> GetAllPossibleRegTypesByWebinarId(int idWebinar);
        IEnumerable<Order> GetOrdersByEmail(string email, int aff);
        Order FindExpressCheckoutOrder(string email, int idWebinar);
        IEnumerable<Order> GetOrdersByLastName(string lastName, int aff);
        Order GetOrderById(int id);
        Order GetOrderByIdThin(int id);
        IEnumerable<int> GetOrderIdsByPartialId(int id);
        IList<Order> GetOrdersByUserId(int id);
        IEnumerable<Order> GetOrdersAll(int idAffliate, out int totalNumberOrders);
        IEnumerable<Order> GetOrdersAllForInvoice(int idAffliate, out int totalNumberOrders);
        IEnumerable<Discount> GetSubscriptionsAll(int idAffliate, out int totalNumberOrders);
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
        Discount GetDiscountByOrderId(int idOrder);
        decimal GetAdditionalLocationsPricing(int idWebinar);
        Discount GetDiscountByUser(WebUser currentUser);
        void GetJoinUrl(OrderRow row);
        Discount ApplyDiscountCode(string code, OrderRow row);
        Order GenerateRegistrantKey(Order order);
        int GetNumberOfOrdersPerWebinar(int id);
        void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation);

        Discount GetDiscountById(int discount);

        IList<Order> GetV3OrdersByWebinar(int idWebinar);
        DateTime CalculatePostEventMaterialsAccessExpiry(OrderRow row);
        //void SendOrderToLegacy(Order newOrder);
        WebUser GetWebUserWithAddressAndInstitution(int idUser);
        IEnumerable<int> GetUserIdsByPartialId(int value);
        //object SearchRegistrations(int affiliateID, IList<int> excludeUserIDs, int skip, int take, string search);

        bool VerifyWebUserExists(int idUser);
        Webinar GetWebinarByJoinCode(string joinCode);
        void LoadWebinarIntoOrderRow(OrderRow newOrderRow);
        void SetUserStatusToUnChanged(WebUser user);
        void SetAffiliateStatusToUnChanged(Affiliate affiliate);
        //IEnumerable<Order> GetOrdersByEmailDomain(string email, int aff);
        void SendAdhocNotification(string emails, string subject, string body);

        //void SynchOrders(int webinarId);
        Order FindExpressCheckoutOrderByOrderId(int q11Orderid);
        IEnumerable<Order> GetV3OrdersByWebinarForPostEventClaims(int idWebinar);
        void UpdateOrderByAdmin(Order newOrder);

        PostEventClaim FindPostEventClaimByOnDemandCode(Order order);
        IList<Order> GetV3OrdersByOnDemandClaim();
        IList<PostEventClaim> FindAllPostEventClaims();
        Webinar GetWebinarById(int webinarId);
        void UpdateUserDetails(WebUser user, string firstName, string lastName, string email, string institution, Address billingAddress, Address shippingAddress);
        string GetOnDemandClaimByCode(string hasVal);
        string GetOnDemandClaimById(int idOrder);
        Order GetOrderByOnDemandClaim(string onDemandCode);
        void UpdateDiscountDetails(Discount discount);
        void UpdateShippingAddressDetails(Address shippingAddress, int idUser);
        //string InsertOnDemandClaim(int orderId);

        RegType GetRegTypeByLabel(string regType, int idWebinar);
        bool OnDemandCodeIsUnique(string onDemandCode);
        IList<int> GetV3OrdersIdsByWebinar(int idWebinar);
        List<Order> GetOrdersByWebinar(int idWebinar);
        List<Order> GetOrdersByWebinarForInvoice(int idWebinar);

        void RestoreToDiscount(int newOrderRowId);
        void RemoveFromDiscount(int newOrderRowId);
        IList<Order> GetOrdersByDiscount(int idDiscount);
        Discount CalculateDiscountRedemption(Discount userHasDiscount, OrderRow row);

        IList<WebUser> GetWebUsersOfDiscount(int idDiscount);
        string CheckOrderComments();
        decimal CalculateCreditsRemain(Discount userDiscount);
        decimal CalculateCreditsUsed(Discount userDiscount);
        
        int CheckIfEmailAlreadyRegisteredForWebinar(int idWebinar, string email);
        bool UserHasMultipleEvents(int idUser);
        Order GetOrderByJoinCode(string joinCode);
        List<Registrant> GetCitrixRegistrantsByWebinar(int webinarId);
        IList<Order> GetOrdersByDomain(string searchTerm, int affiliateId, out int totalNumberOrders);

        //string SetOnDemandClaimById(int myRowIdOrder);

        
        string OrderHasCc(Order order);
        Order UserHasPrexistingOrder(Order existingOrder);

        IDictionary<RegType, bool> GetOptionsAvailableToExistingOrder(int id, bool b, Order modelOrder);
        string RemoveDiscountCode(string code, OrderRow row);
        //string CreateCompliancePerspectivesSubscription(OrderRow row, OrderRow row);
        Discount CreateWspCode(Order modelOrder);
    }
}
