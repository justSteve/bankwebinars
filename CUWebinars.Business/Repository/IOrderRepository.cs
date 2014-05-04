using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository
    {
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        //Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, IList<RegType> options);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, RegType registrationType);
        AdditionalLocation CreateAdditionalLocation(string email, decimal price,string fullName);
        Order AssignAffiliate(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        Order FindOrderByIdWithOrderRows(int id);
        IList<Order> FindOrdersByUserIdWithOrderRows(int userId);
        IList<Order> GetOrdersForLiveNotifications(int idWebianr);
        Order SaveOrderChanges(Order order);
        //IDictionary<RegType, Order> SelectOrdersWithScheduledWebinars(int idUser);
        IList<Order> Test(int idUser);
        IList<Order> SelectOrdersWithScheduledWebinars(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
    }
}