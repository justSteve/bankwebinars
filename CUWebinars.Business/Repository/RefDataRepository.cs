using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class RefDataRepository : IRefDataRepository
    {
        public IQueryable<Institution> GetInstitutions()
        {
            using (var context = new TTSWebinarsContext())
            {
                return context.Institutions;
            }
        }

        //TODO: Please verify that this is the correct extension point
        //  Was working on a method that needed to work against all Users and put this
        //  this here because I was patterning my approach off the GetInstitution code.
        //  This question relates to another 'TODO' re: the base repository's GetAll featue
        //
        public IQueryable<WebUser> GetWebUsers()
        {
            using (var context = new TTSWebinarsContext())
            {
                return context.WebUsers;
            }
        }


        public int GetMaxWebUserId()
        {
            try
            {
                using (var context = new TTSWebinarsContext())
                {
                    return context.WebUsers.Max(u => u.idUser);
                }
            }
            catch (Exception)
            {
                return 1000;
            }
        }

        public WebUser GetWebUserByEmail(string email)
        {
            using (var context = new TTSWebinarsContext())
            {
                return context.WebUsers
                    .Include("Addresses")
                    .Include("Institution")
                    .Where(w => w.email == email).SingleOrDefault();
            }
        }

        public IEnumerable<Address> GetAddressesForUser(int id)
        {
            using (var context = new TTSWebinarsContext())
            {
                return context.WebUsers
                    .Where(w => w.idUser == id)
                    .Single()
                    .Addresses;
            }
        }
    }
}
