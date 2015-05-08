using System;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class PostEventPublishModel
    {
        public Order Order { get; set; }
        public String CostFor6month { get; set; }
        public String CostForCD { get; set; }
        public String ExpiryDate { get; set; }
        public String OnDemandCode { get; set; }
        public String OrderSummaryString { get; set; }


        //public OrderGenesis OrderGenesis { get; set; }
        //public bool UserCreatedInCart { get; set; }
        //public bool UserCreatedOnImport { get; set; }
    }
}