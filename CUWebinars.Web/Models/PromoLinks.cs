using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class PromoLinks
    {

        public Affiliate Affiliate { get; set; }

        public IList<Uri> Links { get; set; }

    }
}