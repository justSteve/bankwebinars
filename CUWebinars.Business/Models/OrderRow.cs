using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OrderRow
    {
        public OrderRow()
        {
            this.OrderRowOptions = new List<OrderRowOption>();
        }

        public int idOrderRow { get; set; }
        public int idOrder { get; set; }
        public int idWebinar { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RowPrice { get; set; }
        public Nullable<int> idDiscount { get; set; }
        //public decimal DiscountPercentOff { get; set; }
        //public decimal DiscountFlatOff { get; set; }
        public string AlternateEmail { get; set; }
        public int RegistrationType { get; set; }
        public OrderRowStatus Status { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public bool isAdHocRecording { get; set; }
        public Nullable<decimal> Royalty { get; set; }
        public virtual Order Order { get; set; }
        public virtual Webinar Webinar { get; set; }
        public virtual ICollection<OrderRowOption> OrderRowOptions { get; set; }
    }
}
