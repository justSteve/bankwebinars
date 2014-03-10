using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository
    {
        Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow);
        //Order CreateOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, IList<Option> options);
        OrderRow CreateOrderRow(Webinar webinar, OrderRowOption orderRowOption, string alternateEmail, int registrationType);
        OrderRowOption CreateOrderRowOption(
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