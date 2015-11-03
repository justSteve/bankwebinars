using System.Collections;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IDataTablesService
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, out int totalNumberOrders);
        IEnumerable<Order> GetOrdersByUser(string email, int idAffliate, out int totalNumberOrders);
        //IEnumerable<Order> GetOrdersPaged(int start, int length, string orderIdFragment, out int totalNumberOrders, out int totalFilteredOrders);
        IEnumerable<WebUser> GetWebUsers(int idAffliate, out int totalNumberUsers);
        //IEnumerable SearchWebinars(string searchTerm, int idAffiliate, out int totalNumberWebinars);
        IEnumerable<WebUser> GetWebUsers(int idAffliate, int totalNumberUsers, out int totalNumberUsers_);
        IEnumerable<Webinar> SearchWebinars(string searchTerm, int idAffiliate, out int totalNumberWebinars);
    }
}