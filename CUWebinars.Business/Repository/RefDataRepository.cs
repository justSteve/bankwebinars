using CUWebinars.Business.Models;
using System;
using System.Data.Entity;
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
                context.Configuration.ProxyCreationEnabled = false;

                return context.Institutions;
            }
        }

        public IQueryable<WebUser> GetWebUsers()
        {
            //TODO: Please verify that this is the correct extension point
            //  Was working on a method that needed to work against all Users and put this
            //  this here because I was patterning my approach off the GetInstitution code.
            using (var context = new TTSWebinarsContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                return context.WebUsers;
            }
        }


        public int GetMaxWebUserId()
        {
            try
            {
                using (var context = new TTSWebinarsContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;
                    return context.WebUsers.Max(u => u.idUser);
                }
            }
            catch (Exception)
            {
                return 1000;
            }
        }

        public IList<Order> FindOrdersByUserId(int id)
        {
            using (var context = new TTSWebinarsContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                var userOrders = context.Orders.Include(o => o.OrderRows.Select(or => or.Webinar)).Where(o => o.WebUser.idUser == id);

                return ReferenceEquals(null, userOrders) ? null : userOrders.ToList();
            }
        }

        public WebUser GetWebUserByEmail(string email)
        {
            using (var context = new TTSWebinarsContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
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
                context.Configuration.ProxyCreationEnabled = false;
                return context.WebUsers
                    .Where(w => w.idUser == id)
                    .Single()
                    .Addresses;
            }
        }

        public IList<RegType> FindRegTypesByWebinarId(int id, bool detached)
        {
            using (var context = new TTSWebinarsContext())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var webinars = context.Webinars
                    .Include(w => w.RegTypesGroupsXref.Select(o => o.RegTypesGroup.RegTypesXrefs.Select(ox => ox.RegType)))
                    .Where(w => w.idWebinar == id)
                    .ToList();

                // There can be only one OptionsGroupsXrefs per webinar at any one time
                var optionsGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

                //  For each of those OptionsGroupsXrefs, get the relevant OptionGroup
                var optionsGroups = optionsGroupsXrefs.Select(o => o.RegTypesGroup);

                //  Get all OptionsXrefs for those OptionGroups
                var optionsXrefs = optionsGroups
                    .SelectMany(opt => opt.RegTypesXrefs);

                //  Finally, get the options
                var options = optionsXrefs.Select(o => o.RegType).ToList();

                if (!detached)
                    return options;

                options.ForEach(o => context.Entry(o).State = EntityState.Detached);

                return options;
            }
        }
    }
}
