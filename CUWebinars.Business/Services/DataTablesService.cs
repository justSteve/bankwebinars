using CUWebinars.Business.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Services
{
    public class DataTablesService : IDataTablesService
    {
        private readonly TTSWebinarsContext _context;
        private readonly IWebinarManagementService _webinarManagementService;
        //private readonly IDataTablesService _dataTablesService;

        public DataTablesService(TTSWebinarsContext context, IWebinarManagementService webinarManagementService)
        {
            _webinarManagementService = webinarManagementService;
            _context = context;
            //_dataTablesService = dataTablesService;
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
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
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
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && or.idWebinar == idWebinar))
                    .ToList();
            }

            totalNumberOrders = theseOrders.Count;

            return theseOrders;
        }

        public IEnumerable<Order> GetOrdersByUser(string email, int idAffliate, out int totalNumberOrders)
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
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && o.BillingEmail == email) && o.idAffiliate == idAffliate)
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
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && o.BillingEmail == email))
                    .ToList();
            }

            totalNumberOrders = theseOrders.Count;

            return theseOrders;
        }

        //public IEnumerable<Order> GetOrdersPaged(int start, int length, string orderIdFragment, out int totalNumberOrders, out int totalFilteredOrders)
        //{
        //    totalNumberOrders = _context.Orders.Count();

        //    var filteredResult = _context.Orders.Where(o => o.idOrder.ToString().ToLower().Contains(orderIdFragment));
        //    totalFilteredOrders = filteredResult.Count();

        //    return filteredResult.OrderByDescending(order => order.idOrder).Skip(start).Take(length); ;
        //}

        public IEnumerable<WebUser> GetWebUsers(int idAffliate, out int totalNumberUsers)
        {
            IList<WebUser> theseUsers;

            if (idAffliate != 19)
            {
                theseUsers = _context.WebUsers
                    .Include(u => u.Institution)
                    .Include(u => u.Orders)
                    .Include(u => u.Addresses)
                    .Take(200).ToList();
            }
            else
            {
                theseUsers = _context.WebUsers.Where(u => u.idUser > 37000).Include(u => u.Institution)
                    .Include(u => u.Orders)
                    .Include(u => u.Addresses)
                    .Take(200)
                    .ToList();
            }

            totalNumberUsers = theseUsers.Count;

            return theseUsers;
        }

        public IEnumerable<Webinar> SearchWebinars(string searchTerm, int idAffiliate, out int totalNumberWebinars)
        {
            
            List<Webinar> webinars = _webinarManagementService.GetSearchDTO(searchTerm).ToList();

            totalNumberWebinars = webinars.Count;

            return webinars;
            
        }
    }
}
