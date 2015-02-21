using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository : IDisposable
    {
        Order AttachItem(Order item);
        Order AssignAffiliate(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullName);
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, string origin = null);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType);
        void DeleteOrder(int orderId);
        Order FindOrderByIdWithOrderRows(int id);
        Order FindById(int id);
        IList<Order> FindOrdersByUserId(int userId);
        IList<int> FindOrderIdsByPartialId(int userId);
        IList<Order> FindOrdersByUserIdWithOrderRows(int userId);
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
        //IList<AdditionalLocation> GetAdditionalLocations(int idOrder);
        Discount FindDiscountById(int id);
        Discount FindDiscountByCode(string discountCode);
        Discount FindDiscountByUser(WebUser currentUser);
        int GetNumberOfOrdersPerWebinar(int id);
    }
}