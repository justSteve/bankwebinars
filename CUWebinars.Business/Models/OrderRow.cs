using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class OrderRow
    {
        public OrderRow()
        {
            
        }
        public int idOrder { get; set; }
        public int idOrderRow { get; set; }
        public int idRegType { get; set; }
        public int idWebinar { get; set; }

        public DateTime? AccessExpires { get; set; }
        public string CitrixJoinUrl { get; set; }
        public Discount Discount { get; set; }
        public string OnDemandCode { get; set; }
        public decimal? PercentPaid { get; set; }
        public string RegistrantKey { get; set; }
        public RegType RegistrationType { get; set; }
        public decimal RowPrice { get; set; }
        public OrderRowStatus RowStatus { get; set; }
        public decimal Royalty { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string TtsJoinUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Tax { get; set; }

        public virtual ICollection<AdditionalLocation> AdditionalLocation { get; set; }
        public virtual Order Order { get; set; }
        public bool? SendHardcopy { get; set; }
        public virtual Webinar Webinar { get; set; }
    }
}
