using System.Collections.Generic;
using System.IdentityModel.Claims;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ClaimsAuditViewModel
    {
        public IEnumerable<UserClaim> UserClaims { get; set; }
        public OrderRow Row { get; set; }
    }
}