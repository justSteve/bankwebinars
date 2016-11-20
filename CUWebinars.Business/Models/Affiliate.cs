using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Affiliate
    {
        public Affiliate()
        {
            Orders = new List<Order>();
        }

        public int idUserAff { get; set; }
        public byte CommissionModel { get; set; }
        public string URL { get; set; }
        public string WebBanner { get; set; }
        public string WebFooter { get; set; }
        public string EmailBanner { get; set; }
        public string EmailFooter { get; set; }
        public string ttsDomain { get; set; }
        public string GAPass { get; set; }
        public string supportEmail { get; set; }
        public string DisplayTitle { get; set; }
        public string BillingModel { get; set; }
        public string Logo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactFax { get; set; }
        public string ContactAddress { get; set; }
        public string TechEmail { get; set; }
        public string TechPhone { get; set; }
        public string TechName { get; set; }
        public string EmailPromo { get; set; }
        public string NotiPromos { get; set; }
        public string NotiOrders { get; set; }
        public string NotiInvoices { get; set; }
        
        public virtual WebUser WebUser { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        
        
    }
}
