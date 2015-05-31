using System;
using System.Linq;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IDataTablesService
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders);
        IEnumerable<Order> GetOrdersByUser(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders);
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

        public IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders)
        {

            totalNumberOrders = _context.Orders.Count();

            return _context.Orders.OrderByDescending(order => order.idOrder).Skip(start).Take(length);
        }

        public IEnumerable<Order> GetOrdersByUser(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders)
        {
            throw new NotImplementedException();
        }
    }
}
