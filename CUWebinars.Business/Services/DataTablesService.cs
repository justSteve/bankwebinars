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

        public IEnumerable<WebUser> GetAllWebUsers()
        {
            return _context.WebUsers;
        }

        public IEnumerable<Order> GetOrdersByWebinar(int idWebinar, int idAffliate, out int totalNumberOrders)
        {
            IList<Order> theseOrders;

            if (idAffliate != 19)
            {
                theseOrders = _context.Orders

                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
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
                    .Include(o => o.WebUser.Institution)
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
        //ErrorResponseCMD logs Controller: images | Action: sidebar-list-icon.png
        public IEnumerable<Order> GetOrdersByUser(string email, int idAffliate, out int totalNumberOrders)
        {
            IList<Order> theseOrders;

            if (idAffliate != 19)
            {
                theseOrders = _context.Orders

                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
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
                    .Include(o => o.WebUser.Institution)
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
        public IEnumerable<Order> GetOrdersByPending(int idAffliate, out int totalNumberOrders)
        {
            IList<Order> theseOrders;

            if (idAffliate != 19)
            {
                theseOrders = _context.Orders

                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderStatus == OrderStatus.AwaitingVerification && o.idAffiliate == idAffliate)
                    .ToList();
            }
            else
            {
                theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderStatus == OrderStatus.AwaitingVerification)
                    .ToList();
            }

            totalNumberOrders = theseOrders.Count;

            return theseOrders;
        }

        public IEnumerable<Order> GetOrdersByDomain(string email, int idAffliate, out int totalNumberOrders)
        {
            IList<Order> theseOrders;
            if (idAffliate != 19)
            {
                theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && o.BillingEmail.EndsWith(email)) && o.idAffiliate == idAffliate)
                    .ToList();
            }
            else
            {
                theseOrders = _context.Orders
                    .Include(o => o.WebUser)
                    .Include(o => o.WebUser.Institution)
                    .Include(o => o.Affiliate)
                    .Include(o => o.OrderRows.Select(or => or.AdditionalLocation))
                    .Include(o => o.OrderRows.Select(or => or.RegistrationType))
                    .Include(o => o.OrderRows.Select(or => or.Discount))
                    .Include(o => o.OrderRows.Select(or => or.Webinar))
                    .Where(o => o.OrderRows.Any(or => or.RowStatus == OrderRowStatus.Active && o.BillingEmail.EndsWith(email)))
                    .ToList();
            }

            totalNumberOrders = theseOrders.Count;

            return theseOrders;
        }

        public IEnumerable<WebUser> GetWebUsers(int idAffliate, out int totalNumberUsers)
        {
            IList<WebUser> theseUsers;

            if (idAffliate != 19)
            {
                theseUsers = _context.Orders
                    .Where(o => o.idAffiliate == idAffliate)
                    .Select(o => o.WebUser).Distinct()
                    .Include(o => o.Institution)
                    .Include(o => o.Orders)

                    .Where(u => u.UserType == UserType.Customer)
                    .ToList();
            }
            else
            {
                theseUsers = _context.Orders
                    .Select(o => o.WebUser).Distinct()
                    .Include(o => o.Institution)
                    .Include(o => o.Orders)
                    .Where(u => u.UserType == UserType.Customer)
                    .ToList();
                //theseUsers = _context.WebUsers
                //    .Include(u => u.Institution)
                //    .Include(u => u.Orders)
                //    .Include(u => u.Addresses)
                //    .Where( u => u.UserType == UserType.Customer)
                //    .Take(200)
                //    .ToList();
            }

            totalNumberUsers = theseUsers.Count;

            theseUsers.Take(500);

            return theseUsers;
        }

        public IEnumerable<Webinar> SearchWebinars(string searchTerm, int idAffiliate, out int totalNumberWebinars)
        {

            List<Webinar> webinars = _webinarManagementService.GetSearchDTO(searchTerm).ToList();

            totalNumberWebinars = webinars.Count();

            return webinars;

        }


        public IEnumerable<WebUser> GetWebUsers(int idAffliate, int totalNumberUsers, out int totalNumberUsers_)
        {
            IEnumerable<WebUser> theseUsers;

            if (idAffliate != 19)
            {
                theseUsers = _context.WebUsers
                    .Include(u => u.Institution)
                    .Include(u => u.Orders.Where(o => o.idAffiliate == idAffliate))
                    .Include(u => u.Addresses)

                    .ToList();
            }
            else
            {
                theseUsers = _context.WebUsers
                    .Include(u => u.Institution)
                    .Include(u => u.Orders)
                    .Include(u => u.Addresses)
                    .ToList();
            }

            totalNumberUsers_ = theseUsers.Count();

            return theseUsers;
        }
    }
}
