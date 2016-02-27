using System.Collections.Generic;
using System.Security.Policy;

namespace CUWebinars.Business.Models
{
    public class OrderRowModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string Price { get; set; }
        public string Percent { get; set; }
        public string Royalty { get; set; }
        public string Status { get; set; }
        public string OrderID { get; set; }
        public string Discount { get; set; }
    }
}