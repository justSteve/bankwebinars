using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditDiscountModel
    {

        [UIHint("Discount")]
        public DiscountDetailsModel Discount { get; set; }

        [HiddenInput]
        public string EditDiscountTitle { get; set; }

    }
}