using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;

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
            //TODO: review naming convention and clarify under what conditions the relation between an Option and a RegType would ever be 1:N (iow, why's this a list?)
            return items.Where(o => o.idRegType == optionId).ToList();
        }

        public IDictionary<RegType, bool> FindRegTypesByWebinarId(int id, bool detached)
        {
            IDictionary<RegType, bool> regTypesAndShippingRequirement = new Dictionary<RegType, bool>();

            var stronglyTypedContext = (TTSWebinarsContext) db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == id);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time
            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

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