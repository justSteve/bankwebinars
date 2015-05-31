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

        public IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, int start, int length, out int totalNumberOrders)
        {
            //if (idAffliate != 19)
            //{
            //    var theseOrders = _context.Orders
            //        .Include(o => o.WebUser)
            //        .Include(o => o.Affiliate)
            //        .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
            //        .Include(o => o.OrderRows.Select(or => or.RegistrationType))
            //        .Include(o => o.OrderRows.Select(or => or.Discount))
            //        .Where(
            //            OrderRows.Single(
            //                or =>
            //                    or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar &&
            //                    or.idAffliate == idAffliate));

            //    totalNumberOrders = theseOrders.Count();
            //}
            //else
            //{
            //    var theseOrders = _context.Orders
            //        .Include(o => o.WebUser)
            //        .Include(o => o.Affiliate)
            //        .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
            //        .Include(o => o.OrderRows.Select(or => or.RegistrationType))
            //        .Include(o => o.OrderRows.Select(or => or.Discount))
            //        .Where(
            //            OrderRows.Single(
            //                or =>
            //                    or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar ));

            //    totalNumberOrders = theseOrders.Count();
            
            //return theseOrders.OrderByDescending(order => order.idOrder).Skip(start).Take(length);
            
            //}
            totalNumberOrders = 0;
            return null;
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
