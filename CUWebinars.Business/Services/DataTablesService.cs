using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Services
{
    public interface IDataTablesService
    {
        IEnumerable<Order> GetAllOrders();
        IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, out int totalNumberOrders);
        IEnumerable<Order> GetOrdersByUser(int idUser, int idAffliate, int start, int length, out int totalNumberOrders);
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

        public IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, out int totalNumberOrders)
        {
            IList<Order> theseOrders;

            if (idAffliate != 19)
            {
                theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar) && o.idAffiliate == idAffliate)
                    .ToList();
            }
            else
            {
                theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar))
                    .ToList();
            }

            totalNumberOrders = theseOrders.Count;

            return theseOrders;
        }

        public IEnumerable<Order> GetOrdersByUser(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders)
        {
            throw new NotImplementedException();
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
