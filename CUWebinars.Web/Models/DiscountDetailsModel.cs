using System.ComponentModel.DataAnnotations;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class DiscountDetailsModel
    {
        public int UserId { get; set; }
        [UIHint("DiscountDetails")]
        public DiscountModel Discount { get; set; }
    }
}