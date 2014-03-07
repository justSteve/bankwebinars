using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        Order AssignAffiliateToOrder(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        string BuildConnectionInfo(OrderRow orderRow);
        Order CreateNewOrder();
        OrderRow CreateOrderRow(Webinar webinar, Order order, string alternateEmail, int registrationType);
        OrderRowOption CreateOrderRowOption(
            OrderRow orderRow,
            Option option,
            string optionDescription,
            decimal price,
            string alternateEmail,
            int additionalLocationsCount,
            string[] additionalLocationsEmails);
        IList<Option> GetOptionsByWebinarId(int id, bool detached);
        IList<Order> GetOrdersByUserId(int id);
        Webinar GetWebinar(int id);
        WebUser GetWebUser(int id);
        void CreateOrderEvent(Order order, UserAccount userAccount);
        void DispatchDummyOrder();
        OrderRow LoadOrderRow(int id);
        byte GetOrderInitiator();
        void AssignUserToOrder(Order currentOrder, WebUser user);
        void CreateCPSubscription(OrderRow orderRow);
        void Save(Order currentOrder);
        Order SaveOrderChanges(Order currentOrder);
        void AddOrderRow(Order currentOrder, OrderRow orderRow);
    }
}
