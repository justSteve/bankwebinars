using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OrderRow
    {
        public OrderRow()
        {
            //this.AdditionalLocation = new List<AdditionalLocation>();
        }

        public int idOrderRow { get; set; }
        public int idOrder { get; set; }
        public int idWebinar { get; set; }
        public int idRegType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RowPrice { get; set; }
        public decimal Royalty { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public Nullable<System.DateTime> AccessExpires { get; set; }
        public OrderRowStatus RowStatus { get; set; }
        public Discount Discount { get; set; }
        public RegType RegistrationType { get; set; }
        public virtual Order Order { get; set; }
        public virtual Webinar Webinar { get; set; }
        public virtual ICollection<AdditionalLocation> AdditionalLocation { get; set; }
    }
}
