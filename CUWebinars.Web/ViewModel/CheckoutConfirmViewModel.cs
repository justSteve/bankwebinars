
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CheckoutConfirmViewModel
    {
        public string AffiliateComments { get; set; }
        public string AdditionalLocationCaption { get; set; }
        public string AdminComments { get; set; }
        public string CCUserDetails { get; set; }
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
        public string UserComments { get; set; }
        public string UserDetails { get; set; }
        public UserType UserType { get; set; }
    }
}