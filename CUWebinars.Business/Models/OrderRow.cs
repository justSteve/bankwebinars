using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OrderRow
    {
        public OrderRow()
        {
            //this.AdditionalLocations = new List<AdditionalLocations>();
        }

        public int idOrderRow { get; set; }
        public int idOrder { get; set; }
        public int idWebinar { get; set; }
        public int idRegType { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RowPrice { get; set; }
        public decimal Royalty { get; set; }
        public OrderRowStatus Status { get; set; }
        public Discount Discount { get; set; }
        public string AlternateEmail { get; set; }
        public RegType RegistrationType { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public Nullable<System.DateTime> AccessExpires { get; set; }
        public virtual Order Order { get; set; }
        public virtual Webinar Webinar { get; set; }
        public virtual AdditionalLocations AdditionalLocation { get; set; }
    }
}
