using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class InvoicesModel
    {

        public Affiliate Affiliate { get; set; }
        public WebUser WebUser { get; set; }
        public IList<Uri> Links { get; set; }

    }
}