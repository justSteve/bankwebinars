using System;
using System.Data.Entity;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IWebUserRepository : IDisposable
    {
        WebUser FindById(int id);
        WebUser FindByIdLoaded(int id);
        WebUser GetWebUserByEmail(string email);
        IEnumerable<WebUser> GetWebUsersByLastName(string lastName);
        WebUser GetWebUserByEmailLoadedWithOrdersData(string email);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        IEnumerable<WebUser> GetWebusersForWebinarWithRegtypes(int idWebinar, IEnumerable<int> regTypeIds);
        void Add(WebUser webUser);
        IEnumerable<WebUser> GetAll();
        void UpdateAddresses(Address address);
        void Update(WebUser webUser);

        DbContext DbContext { get; }
    }
}
