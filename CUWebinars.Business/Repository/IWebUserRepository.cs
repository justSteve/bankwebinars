using BrockAllen.MembershipReboot;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IWebUserRepository : IRepository<WebUser>
    {
        void UpdateAddresses(Address address);
    }
}
