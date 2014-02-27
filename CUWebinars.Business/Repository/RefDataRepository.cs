using System.Runtime.Remoting.Contexts;
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

        public IList<Option> FindOptionsByWebinarId(int id)
        {
            using (var context = new TTSWebinarsContext())
            {
                var webinars = context.Webinars
                    .Where(w => w.idWebinar == id).ToList();

                ////  Get all OptionsGroupsXrefs for those webinars
                //
 
                // There can be only one OptionsGroupsXrefs per webinar at any one time
                // - perhaps the pluralization causes confusion
                // but this entity exists solely to let us know what options are valid
                // actually, it's a lexical quibble but more accurate to say:
                //   'which _set_ of options is valid' for
                // the event _at this point in time_. Should we display the set of pre-event (scheduled)
                // options, or the post-event (recorded) options, or expired 
                // (technically that'd be _no options- six months passed we no longer take orders).
                
                //How it looks in the db:
                // for sake of space I'll let you paste this tsql in to review results:
                //  SELECT * FROM dbo.OptionsGroups

                // Shows all the possible Groups (or 'sets') of options.
                // Each given event will have one (and only one) of these values and will
                // progress from the 'Pre' to 'Post' according to given point in time. (then all expire).

                // Add a couple joins: 
                // SELECT og.idOptionGroup FROM dbo.OptionsGroups og INNER JOIN dbo.OptionsGroupsXref ogx 
                //  ON ogx.idOptionGroup = og.idOptionGroup
                //  INNER JOIN dbo.Webinar w ON w.idWebinar = ogx.idWebinar WHERE w.idWebinar = 400

                // Shows us the cross tabulation value is 34. That's found in the OptionsXref table
                // and, again, because it's a cross tab table, we need to add 2 joins to get at any 
                // readable data:

                //  SELECT  og.idOptionGroup, o.OptionLabel FROM dbo.OptionsGroups og INNER JOIN dbo.OptionsGroupsXref ogx ON ogx.idOptionGroup = og.idOptionGroup
                //INNER JOIN dbo.OptionsXref ox ON ox.idOptionGroup = ogx.idOptionGroup
                //INNER JOIN dbo.Options o ON o.idOption = ox.idOption
                //INNER JOIN dbo.Webinar w ON w.idWebinar = ogx.idWebinar WHERE w.idWebinar = 400
                //
                //
                // This is not the method where any concern to 'Orders' needs to take place.
                // This method is only talking to listeners interested in what options 
                // (which set of options) should I show?
                //<< end of steve's insertion>>

                //var optionsGroupsXrefs = webinars.SelectMany(w => w.OptionsGroupsXrefs);
                //var A = optionsGroupsXrefs.ToList();

                ////  For each of those OptionsGroupsXrefs, get the relevant OptionGroup
                //var optionsGroups = optionsGroupsXrefs.Select(o => o.OptionsGroup);
                //var B = optionsGroups.ToList();

                ////  Get all OptionsXrefs for those OptionGroups
                //var optionsXrefs = optionsGroups.SelectMany(opt => opt.OptionsXrefs);
                //var C = optionsXrefs.ToList();

                ////  Finally, get the options
                //var options = optionsXrefs.Select(o => o.Option);                //  Get all OptionsGroupsXrefs for those webinars
                
                var orderRows = webinars.SelectMany(w => w.OrderRows);
                var A = orderRows.ToList();

                //  For each of those OptionsGroupsXrefs, get the relevant OptionGroup
                var orderRowOptions = orderRows.SelectMany(o => o.OrderRowOptions);
                var B = orderRowOptions.ToList();

                //  Get all OptionsXrefs for those OptionGroups
                var options = orderRowOptions.Select(opt => opt.Option);

                return options.ToList();

            }
        }
    }
}
