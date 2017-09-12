using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class AuditDiscountModel
    {
        public int idOrder { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Email { get; set; }
        public decimal Credits { get; set; }
        public string OrderDate { get; set; }
    }
}