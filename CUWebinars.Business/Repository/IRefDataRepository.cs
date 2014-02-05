using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Linq;
namespace CUWebinars.Business.Repository
{
    public interface IRefDataRepository
    {
        IQueryable<Option> FindOptionsByWebinarId(int id);
        IEnumerable<Address> GetAddressesForUser(int id);
        IQueryable<Institution> GetInstitutions();
        WebUser GetWebUserByEmail(string email);        
        IQueryable<WebUser> GetWebUsers();
        int GetMaxWebUserId();
    }
}
