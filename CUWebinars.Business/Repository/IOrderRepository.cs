using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository : IDisposable
    {
        void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation);
        Order AttachItem(Order item);
        Order AssignAffiliate(int affiliateId, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullName);
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        Order CreateOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType);
        void DeleteOrder(Order orderId);
        Order GetOrderById(int id);
        Order FindById(int id);
        IQueryable<Order> FindOrdersByBillingEmail(string email, int aff);
        IQueryable<Order> FindOrdersByBillingEmailDomain(string email, int aff);
        IQueryable<Order> FindOrdersByLastName(string lastName, int idAffiliate);
        IList<Order> FindOrdersByUserId(int userId);
        IList<int> FindOrderIdsByPartialId(int userId);
        IList<Order> FindOrdersByUserIdWithOrderRows(int userId);
        IEnumerable<Order> GetOrdersByUserId(int userId);
        IEnumerable<Order> GetOrdersAll(int idAffliate, out int totalNumberOrders);
        IList<Order> GetOrdersForLiveEventNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedEventNotifications(int idWebinar);
        IList<Order> GetOrdersForShippedEventNotifications();
        OrderRow GetOrderRowById(int idOrderRow);
        int SaveChanges();
        Order SaveOrderChanges(Order order, int? isFromSignup);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        int AccessToPostEventMaterials(int i, int i1);
        Discount FindDiscountById(int id);
        Discount FindDiscountByCode(string discountCode);
        Discount FindDiscountByUser(WebUser currentUser);
        Order GetOrderByIdThin(int idOrder);
        int GetNumberOfOrdersPerWebinar(int id);
        void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation);
        
        IList<int> FindUserIdsByPartialId(int value);
        object SearchOrders(int affiliateId, IList<int> excludeUserIDs, int skip, int take, string search);
        void LoadWebinarIntoOrderRow(OrderRow newOrderRow);
        Order FindExpressCheckoutOrder(string trim, int idWebinar);
        void ConvertLegacyOrder(Order order);
        int MigrateOrderFromV3(Order order);
        Order FindExpressCheckoutOrderByOrderId(int q11Orderid);
        void SynchIds(int lOrder, int vOrder);
        PostEventClaim FindPostEventClaim(Order order);
        IList<Order> GetV3OrdersByOnDemandClaim();
        Order MigrateOrderWithDiscount(Order order);
        Order GetOrderByIdByOnDemandCode(string onDemandCode);
        Discount GetDiscountByOrderId(int idOrder);
        void UpdateShippingAddressDetails(Address shippingAddress, int idUser);
    }
}