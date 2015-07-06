
using System.ComponentModel.DataAnnotations;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CheckoutConfirmViewModel
    {
        public string AffiliateComments { get; set; }
        public string AdditionalLocationCaption { get; set; }
        [UIHint("AdjustUserDetails")]
        public AdjustUserDetailsEditModel AdjustUserDetailsPanel { get; set; }
        public string AdminComments { get; set; }
        public string CCUserDetails { get; set; }
        public string CheckoutDiscountCode { get; set; }
        public DisplayOptionsInDropDownViewModel DisplayOptionsInDropDownViewModel { get; set; }
        public DisplayRowPriceViewModel DisplayRowPriceViewModel { get; set; }
        public int idUser { get; set; }
        public bool OrderExists { get; set; }
        public OrderHasAdditionalLocationsViewModel OrderHasAdditionalLocationsViewModel { get; set; }
        public bool OrderRowExists { get; set; }
        public bool OrderRowHasId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Origin { get; set; }
        public string OptionLabel { get; set; }
        public ShippingDetailsModel ShippingDetailsModel { get; set; }
        public DiscountDetailsModel DiscountDetailsModel { get; set; }
        public string UserComments { get; set; }
        public string UserDetails { get; set; }
        public string UserFullname { get; set; }
        public UserType UserType { get; set; }

        public string T_ordernum { get; set; }
        public string C_company { get; set; }
        public string C_fname { get; set; }
        public string C_lname { get; set; }
        public string C_address { get; set; }
        public string C_city { get; set; }
        public string C_state { get; set; }
        public string C_zip { get; set; }
        public string C_email { get; set; }
        public string C_telephone { get; set; }

    }
}