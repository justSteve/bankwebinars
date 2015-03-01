using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public abstract class OrderSumUserViewModel
    {
        public Order Order { get; set; }
    }
}