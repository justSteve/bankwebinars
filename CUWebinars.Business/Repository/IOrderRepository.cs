using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository
    {
        Order AssignAffiliate(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price, string fullName);
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType);
        Order FindOrderByIdWithOrderRows(int id);
        IList<Order> FindOrdersByUserIdWithOrderRows(int userId);
        IList<Order> GetOrdersForLiveEventNotifications(int idWebinar);
        IList<Order> GetOrdersForRecordedEventNotifications(int idWebinar);
        IList<Order> GetOrdersForShippedEventNotifications();
        Order SaveOrderChanges(Order order);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        IList<Order> Test(int idUser);
        int CheckUserForRecordingAccess(int i, int i1);
    }
}