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
        int? GetWebUserIdByEmail(string email);
        string GetWebUserFullname(string email);
        IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate);
        WebUser GetWebUserByEmailLoadedWithOrdersData(string email);
        IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar);
        IEnumerable<WebUser> GetWebusersForWebinarWithRegtypes(int idWebinar, IEnumerable<int> regTypeIds);
        void Add(WebUser webUser);
        IEnumerable<WebUser> GetAll();
        void UpdateAddresses(Address address);
        void Update(WebUser webUser);
        bool WebUserExists(int idUser);

        DbContext DbContext { get; }
        WebUser GetWebUserByIdLoadedWithAddressesAndInstitution(int idUser);
    }
}
