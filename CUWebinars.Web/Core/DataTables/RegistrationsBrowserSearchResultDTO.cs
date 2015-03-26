using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core.DataTables
{
    public class RegistrationsBrowserSearchResultDTO
    {
        public int TotalCount { get; set; }
        public int FoundCount { get; set; }

        private IList<Order> _orders = new List<Order>();
        public IList<Order> Orders
        {
            get { return _orders; }
            set { _orders = value; }
        }

        private IList<OrderRow> _orderRows = new List<OrderRow>();
        public IList<OrderRow> OrderRows
        {
            get { return _orderRows; }
            set { _orderRows = value; }
        }


    }
}