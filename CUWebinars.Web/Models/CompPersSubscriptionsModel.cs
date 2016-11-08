using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class CompPersSubscriptionsModel
    {
        public IList<DiscountDTO> Discounts { get; set; }
        public List<WebUser> WebUsers { get; set; }
    }
}