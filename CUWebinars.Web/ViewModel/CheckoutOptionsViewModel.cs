
using System.Collections.Generic;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class CheckoutOptionsViewModel
    {
        public IEnumerable<AdditionalLocation> AdditionalLocations { get; set; }
        public bool ConnectionInfoPresent { get; set; }
        public DisplayOptionsViewModel DisplayOptionsViewModel { get; set; }
        public int idUser{ get; set; }
        public int idWebinar { get; set; }
        //public int idAffiliate { get; set; }
        public bool OrderExists { get; set; }
        public bool OrderHasId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int RegistrationTypeId { get; set; }
        public RegType RegistrationType { get; set; }
        /// <summary>
        /// This property only relevant for Affiliate and Admin cart checkouts.
        /// </summary>
        [HiddenInput]
        
        public int idOrder { get; set; }
        public int SelectedWebUser { get; set; }
        public decimal WebinarDuration { get; set; }
        public WebinarStatus WebinarStatus { get; set; }
    }
}