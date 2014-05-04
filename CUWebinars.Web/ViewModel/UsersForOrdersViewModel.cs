using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Controllers
{
    public class UsersForOrdersViewModel
    {
        public IEnumerable<WebUser> WebUsers { get; set; }
    }
}