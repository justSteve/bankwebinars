using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        Order AssignAffiliateToOrder(Affiliate affiliate, Order order);
        void AssignUserToOrder(Order currentOrder);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        string BuildConnectionInfo(OrderRow orderRow);
        OrderRow CreateOrderRow(Webinar webinar, OrderRowOption orderRowOption, string alternateEmail, int registrationType);
        OrderRowOption CreateOrderRowOption(
            Option option,
            string optionDescription,
            decimal price,
            string alternateEmail,
            int additionalLocationsCount,
            string[] additionalLocationsEmails);
        IList<Option> GetOptionsByWebinarId(int id, bool detached);
        IList<Option> GetOptionsByWebinarIdFromOptionsRepository(int id, bool detached);
        Order GetOrderById(int id);
        IList<Order> GetOrdersByUserId(int id);
        Webinar GetWebinar(int id);
        WebUser GetWebUser(int id);
        void CreateOrderEvent(Order order, UserAccount userAccount);
        void DispatchDummyOrder();
        OrderRow LoadOrderRow(int id);
        byte GetOrderInitiator();
        void CreateCPSubscription(OrderRow orderRow);
        //void Save(Order currentOrder);
        Order SaveOrderChanges(Order currentOrder);
        void AddOrderRow(Order currentOrder, OrderRow orderRow);


        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
//        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, IList<Option> options);
    }
}
