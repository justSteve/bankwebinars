using System;
using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository : IDisposable
    {
        void AddAdditionalLocation(AdditionalLocation addedAdditionalLocation);
        Order AttachItem(Order item);
        Order AssignAffiliate(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullName);
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        Order CreateOrder(int affiliateId, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType);
        void DeleteOrder(int orderId);
        Order FindOrderByIdWithOrderRows(int id);
        Order FindById(int id);
        IQueryable<Order> FindOrdersByBillingEmail(string email);
        IQueryable<Order> FindOrdersByLastName(string lastName);
        IList<Order> FindOrdersByUserId(int userId);
        IList<int> FindOrderIdsByPartialId(int userId);
        IList<Order> FindOrdersByUserIdWithOrderRows(int userId);
        IEnumerable<Order> GetOrdersByUserId(int userId);
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
        Order GetOrderById(int idOrder);
        int GetNumberOfOrdersPerWebinar(int id);
        void RemoveAndDeleteAdditionalLocation(AdditionalLocation deletedAdditionalLocation);
        void SendOrderToLegacy(Order newOrder);
        IList<int> FindUserIdsByPartialId(int value);
        object SearchOrders(int affiliateId, IList<int> excludeUserIDs, int skip, int take, string search);
        void LoadWebinarIntoOrderRow(OrderRow newOrderRow);
    }
}