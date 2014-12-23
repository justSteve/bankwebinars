using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class ImportOrderModel
    {
        public string AdditionalLocationsString { get; set; }

        public string AffiliateComments { get; set; }
        
        public int idWebinar { get; set; }
        public string RegistrationType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Institution { get; set; }
        public string Email { get; set; }
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public int idAffiliate { get; set; }
        public string DiscountCode { get; set; }
        public string AdditionalLocations { get; set; }

    }

}