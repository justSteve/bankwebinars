using System.Collections.Generic;
using System.IdentityModel.Claims;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Services;

namespace CUWebinars.Web.ViewModel
{
    public class ClaimsViewModel
    {
        public IEnumerable<UserClaim> UserClaims { get; set; }
        public string ClaimToDelete { get; set; }
    }
}