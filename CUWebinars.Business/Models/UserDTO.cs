using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Models
{
    public class UserDTO
    {
        public int idOrder { get; set; }
        public int idOrderLegacy { get; set; }
        public int idUser { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateString { get { return OrderDate.ToShortDateString(); } } // to ease consuption in the JS / DataTables caller
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusString { get { return OrderStatus.ToString(); } } // to ease consuption in the JS / DataTables caller
        public decimal Total { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Institution { get; set; }
        public string BillingEmail { get; set; }

        public string Affiliate_ttsDomain { get; set; } // flattened via Automapper to avoid circular reference during JSON serialization


        // from active OrderRow object which was flattened via Automapper to avoid circular reference during JSON serialization
        public string TtsJoinUrl { get; set; }
        public Discount Discount { get; set; }
        public RegType RegistrationType { get; set; }

        public bool Webinar_IsActive { get; set; } // flattened via Automapper to avoid circular reference during JSON serialization

    }
}