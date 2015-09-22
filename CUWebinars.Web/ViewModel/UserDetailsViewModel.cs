using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class UserDetailsViewModel
    {
        public List<WebUser> WebUsers { get; set; }
        public List<Order> Orders { get; set; }
        public bool UserIsAdmin { get; set; }
        public Affiliate Affiliate { get; set; }
    }
}