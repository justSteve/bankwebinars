using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class AdminDTO
    {
        public IList<OrderRow> OrderRows = new List<OrderRow>();
        public IList<WebUser> Users = new List<WebUser>();
    }

}