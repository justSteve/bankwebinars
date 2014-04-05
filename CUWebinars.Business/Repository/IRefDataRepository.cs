using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Linq;
namespace CUWebinars.Business.Repository
{
    public interface IRefDataRepository
    {
        //IList<RegType> FindRegTypesByWebinarId(int id, bool detached);
        IEnumerable<Address> GetAddressesForUser(int id);
        IQueryable<Institution> GetInstitutions();
        WebUser GetWebUserByEmail(string email);        
        IQueryable<WebUser> GetWebUsers();
        int GetMaxWebUserId();
        IList<Order> FindOrdersByUserId(int id);
    }
}
