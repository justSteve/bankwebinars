using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository
    {
        Order CreateOrder();
        OrderRow CreateOrderRow(Webinar webinar, Order order, string alternateEmail, int registrationType);
        OrderRowOption CreateOrderRowOption(
            OrderRow orderRow,
            Option option,
            string optionDescription,
            decimal price,
            string alternateEmail,
            int additionalLocationsCount,
            string[] additionalLocationsEmails);
        Order AssignAffiliate(Affiliate affiliate, Order order);
        Order AssignWebUserToOrder(WebUser webUser, Order order);
        Order SaveOrderChanges(Order order);
        IDictionary<Option, Order> SelectOrdersWithScheduledWebinars(int idUser);
        IList<Order> Test(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
    }
}