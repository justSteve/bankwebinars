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
        IEnumerable<Order> GetOrdersByUser(int idUser, int idAffliate, int start, int length, out int totalNumberOrders);
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
            if (idAffliate != 19)
            {
                var theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Where(
                        OrderRows.Single(
                            or =>
                                or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar &&
                                or.idAffliate == idAffliate));

                totalNumberOrders = theseOrders.Count();
            }
            else
            {
                var theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Where(
                        OrderRows.Single(
                            or =>
                                or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar ));

                totalNumberOrders = theseOrders.Count();
            
            return theseOrders.OrderByDescending(order => order.idOrder).Skip(start).Take(length);
            
            }
        }

        public IEnumerable<Order> GetOrdersByUser(int idUser, int idAffliate, int start, int length, out int totalNumberOrders)
        {
            _context.Orders = _context.Orders.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);

            totalNumberOrders = _context.Orders.Count();

            return _context.Orders.OrderByDescending(order => order.idOrder).Skip(start).Take(length);
        }
    }
}
