using System.Data.Entity;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IWebUserRepository
    {
        WebUser FindById(int id);
        WebUser FindByIdLoaded(int id);
        WebUser GetWebUserByEmail(string email);
        void Add(WebUser webUser);
        IEnumerable<WebUser> GetAll();
        void UpdateAddresses(Address address);
        void Update(WebUser webUser);

        DbContext DbContext { get; }
    }
}
