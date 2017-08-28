using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class WpsViewModel 
    {
        public Affiliate Affiliate;
        public IList<DiscountDTO> Discounts { get; set; }
        public List<WebUser>  WebUsers { get; set; }
    }
}