using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class CompPersSubscriptionsModel
    {

        public int UserId { get; set; }
        [UIHint("CompPersSubscriptionEditor")]
        //public DiscountDetailsModel Discount { get; set; }

        [HiddenInput]
        public string EditDiscountTitle { get; set; }
        public DiscountModel Discount { get; set; }
    }
}