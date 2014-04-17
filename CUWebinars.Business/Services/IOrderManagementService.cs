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
        //OrderRow CreateOrderRow(Webinar webinar, AdditionalLocation AdditionalLocation, RegType registrationType);
        OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocation, int registrationType);

        AdditionalLocation CreateAdditionalLocation(string email,decimal price,string fullname);
        Affiliate GetAffiliateById(int id);
        //RegType GetOptionById(RegType id);
        IList<RegType> GetOptionsByWebinarId(int id, bool detached);
        IList<RegType> GetOptionsByWebinarIdFromOptionsRepository(int id, bool detached);
        Order GetOrderById(int id);
        IList<Order> GetOrdersByUserId(int id);
        Webinar GetWebinar(int id);
        WebUser GetWebUser(int id);
        void CreateOrderEvent(Order order, UserAccount userAccount);
        void DispatchDummyOrder();
        OrderRow LoadOrderRow(int id);
        string GetOrderInitiator();
        void CreateCPSubscription(OrderRow orderRow);
        //void Save(Order currentOrder);
        Order SaveOrderChanges(Order currentOrder);
        void AddOrderRow(Order currentOrder, OrderRow orderRow);


        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
//        Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, IList<RegType> options);
        Discount GetDiscount(string email);
        string CreateRegistrantKey(string firstName, string lastName, string billingEmail, int idWebinar, int webinarKey);
    }
}
