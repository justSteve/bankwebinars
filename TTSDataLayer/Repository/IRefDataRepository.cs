using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
namespace CUWebinars.Business.Repository
{
    public interface IRefDataRepository
    {
        IEnumerable<Address> GetAddressesForUser(int id);
        IQueryable<Institution> GetInstitutions();
        WebUser GetWebUserByEmail(string email);
        WebUser GetWebUserByUserName(string userName);
        IQueryable<WebUser> GetWebUsers();
        int GetMaxWebUserId();
    }
}
