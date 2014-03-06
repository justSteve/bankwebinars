using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOrderRepository
    {
        Order CreateOrder();
        bool AssignAffiliate(Affiliate affiliate, Order order);
        IDictionary<Option, Order> SelectOrdersWithScheduledWebinars(int idUser);
        IList<Order> Test(int idUser);
        IList<Order> SelectOrdersWithRecordedWebinars(int idUser);
        IList<Order> SelectOrdersWithArchivedWebinars(int idUser);
    }
}