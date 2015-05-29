using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Services
{
    public interface IDataTablesService
    {
        IEnumerable<Order> GetAllOrders();
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
    }
}
