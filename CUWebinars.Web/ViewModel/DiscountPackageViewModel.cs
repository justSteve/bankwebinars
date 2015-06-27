using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DiscountPackageViewModel
    {
        public Discount Discount { get; set; }
        public WebUser PrimaryUser { get; set; }
        public IList<WebUser> AuthorizedUsers { get; set; }
        public object NewDiscount { get; set; }
    }
}