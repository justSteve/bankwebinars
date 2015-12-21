using System.Collections.Generic;
using System.IdentityModel.Claims;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Services;

namespace CUWebinars.Web.ViewModel
{
    public class OrderClaimsViewModel
    {
        public IEnumerable<PostEventClaim> OrderClaims { get; set; }
        public string ClaimToDelete { get; set; }
    }
}