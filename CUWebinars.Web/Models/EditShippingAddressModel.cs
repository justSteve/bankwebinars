using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditShippingAddressModel
    {

        [UIHint("Address")]
        public AddressModel ShippingAddress { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }

    }
}