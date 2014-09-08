using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Linq;
namespace CUWebinars.Business.Repository
{
    public interface IRefDataRepository
    {
        IList<Order> FindOrdersByUserId(int id);
        IEnumerable<Address> GetAddressesForUser(int id);
        IEnumerable<Presenter> GetAllPresenters();
        IQueryable<Institution> GetInstitutions();
        Institution GetInstitutionForUser(int id);
        int GetMaxWebUserId();
        IQueryable<WebUser> GetWebUsers();
        WebUser GetWebUserByEmail(string email);
    }
}
