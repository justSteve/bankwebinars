
namespace CUWebinars.Business.Models
{
    public class WebUserDiscountXref
    {
        public int idWebUserDiscount { get; set; }
        public int idDiscount { get; set; }
        public int idWebUser { get; set; }

        public Discount Discount { get; set; }
        public WebUser WebUser { get; set; }
    }
}
