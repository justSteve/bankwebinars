using CUWebinars.Business.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class RegTypeRepository : TTSWebinarsRepository<TTSWebinarsContext, RegType>, IRegTypeRepository
    {
        public RegTypeRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {

        }

        public RegType FindRegType(int id)
        {
            var i = items.FirstOrDefault(o => o.idRegType == id);
            return i;
        }

        public IList<RegType> FindRegTypeOption(int optionId)
        {
            return items.Where(o => o.idRegType == optionId).ToList();
        }
        public IDictionary<RegType, bool> FindAllPossibleRegTypesByWebinarId(int id, bool detached)
        {
            //return every RegType a given event without regard to current state of the event. 
            // permits admins to change a recorded order to 'live' even after the event has occurred
            IDictionary<RegType, bool> regTypesAndShippingRequirement = new Dictionary<RegType, bool>();

            var stronglyTypedContext = (TTSWebinarsContext)db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == id);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time for end-user.
            // But when an Admin is editing an order he needs to see all RegTypes without filtering
            // against the 'PreEvent' or 'PostEvent' state. To support this need a column is added
            // to the RegTypesGroup that provides a CrossRefenence from the current state to it's alternate.

            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

            //  For each of those RegTypesGroupsXrefs, get the relevant OptionGroup
            var regtypesGroups = regTypesGroupsXrefs.Include(o => o.RegTypesGroup)
                .Select(o => o.RegTypesGroup);

            //TODO: Add the RegTypeGroup where idRegTypeGroup = AltState


            //  Get all OptionsXrefs for those RegTypesGroups
            var regtypesXrefs = regtypesGroups.Include(o => o.RegTypesXrefs)
                .SelectMany(opt => opt.RegTypesXrefs);

            //  Finally, get the RegTypes
            //  NOTE: ToDictionary is called here with the "shape" <RegType, bool> because I am pairing the RegType with a bool
            //  that determines whether the RegType in question includes materials to be physically shipped. This "pairing" needs
            //  to make it all the way to the view so when the user clicks on a radio button, that information is right there in the dom.
            var regTypes =
                regtypesXrefs.Include(o => o.RegType)
                    .Select(o => o.RegType)
                    .Distinct()
                    .ToList()
                    .ToDictionary(r => r, rt => rt.ShowShippedNotifications.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase), new RegTypeComparer());

            if (!detached)
                return regTypes;

            foreach (var regType in regTypes)
            {
                stronglyTypedContext.Entry(regType.Key).State = EntityState.Detached;
            }

            return regTypes;

        }
        public IDictionary<RegType, bool> FindRegTypesByWebinarId(int id, bool detached)
        {
            IDictionary<RegType, bool> regTypesAndShippingRequirement = new Dictionary<RegType, bool>();

            var stronglyTypedContext = (TTSWebinarsContext) db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == id);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time
            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);
            
            //when pre-event
            //  For each of those RegTypesGroupsXrefs, get the relevant OptionGroup
            var regtypesGroups = regTypesGroupsXrefs.Include(o => o.RegTypesGroup).Select(o => o.RegTypesGroup);

            //  Get all OptionsXrefs for those RegTypesGroups
            var regtypesXrefs = regtypesGroups.Include(o => o.RegTypesXrefs).SelectMany(opt => opt.RegTypesXrefs);

            //  Finally, get the RegTypes
            var regTypes =
                regtypesXrefs.Include(o => o.RegType)
                    .Select(o => o.RegType)
                    .Distinct()
                    .ToList()
                    .ToDictionary(r => r, rt => rt.ShowShippedNotifications.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase), new RegTypeComparer());

            if (!detached)
                return regTypes;

            foreach (var regType in regTypes)
            {
                stronglyTypedContext.Entry(regType.Key).State = EntityState.Detached;
            }

            return regTypes;
        }



        public bool IsShippingAddressRequired(int regTypeId)
        {
            var regType = items.Find(regTypeId);

            return regType.ShowShippedNotifications.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);
        }
    }


    internal class RegTypeComparer : EqualityComparer<RegType>
    {
        public override bool Equals(RegType x, RegType y)
        {
            return x.idRegType.CompareTo(y.idRegType) == 0;
        }

        public override int GetHashCode(RegType obj)
        {
            return obj.idRegType.ToString().ToLower().GetHashCode();
        }
    }
}