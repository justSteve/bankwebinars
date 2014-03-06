using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        bool AssignAffiliateToOrder(Affiliate affiliate, Order order);
        string BuildConnectionInfo(OrderRow orderRow);
        Order CreateNewOrder();
        IList<Option> GetOptionsByWebinarId(int id);
        IList<Order> GetOrdersByUserId(int id);

        void CreateOrderEvent(Order order, UserAccount userAccount);
        void DispatchDummyOrder();
        OrderRow LoadOrderRow(int id);
        byte GetOrderInitiator();
        void AssignUserToOrder(Order currentOrder, WebUser user);
        void CreateCPSubscription(OrderRow orderRow);
        void Save(Order currentOrder);
        void AddOrderRow(Order currentOrder, OrderRow orderRow);
    }
}
