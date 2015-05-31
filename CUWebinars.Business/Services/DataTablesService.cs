using System.Linq;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IDataTablesService
    {
        IEnumerable<Order> GetAllOrders();

        IEnumerable<Order> GetOrdersPaged(int start, int length, string orderIdFragment, out int totalNumberOrders, out int totalFilteredOrders);
    }

    public class DataTablesService : IDataTablesService
    {
        private readonly TTSWebinarsContext _context;

        public DataTablesService(TTSWebinarsContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders;
        }

        public IEnumerable<Order> GetOrdersPaged(int start, int length, string orderIdFragment, out int totalNumberOrders, out int totalFilteredOrders)
        {
            totalNumberOrders = _context.Orders.Count();
            
            var filteredResult =_context.Orders.Where(o => o.idOrder.ToString().ToLower().Contains(orderIdFragment));
            totalFilteredOrders = filteredResult.Count();

            return filteredResult.OrderByDescending(order => order.idOrder).Skip(start).Take(length); ;
        }
    }
}
