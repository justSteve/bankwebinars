using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IWebUserRepository
    {
        WebUser FindById(int id);
        WebUser FindByIdLoaded(int id);
        void Add(WebUser webUser);
        IEnumerable<WebUser> GetAll();
        void UpdateAddresses(Address address);
        void Update(WebUser webUser);
    }
}
