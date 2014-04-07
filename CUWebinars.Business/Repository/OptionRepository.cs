using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class RegTypeRepository : TTSWebinarsRepository<TTSWebinarsContext, RegType>, IRegTypeRepository
    {
        public RegType FindRegType(RegType id)
        {
            return items.FirstOrDefault(o => o.idRegType.ToString() == id.ToString());
        }

        public IList<RegType> FindRegTypesByWebinarId(int id, bool detached)
        {
            var stronglyTypedContext = (TTSWebinarsContext)db;

            var webinars = stronglyTypedContext.Webinars
                //.Include(w => w.OptionsGroupsXrefs)
                .Where(w => w.idWebinar == id);

            // There can be only one OptionsGroupsXrefs per webinar at any one time
            var optionsGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref );
            //var a = optionsGroupsXrefs.ToList();

            //  For each of those OptionsGroupsXrefs, get the relevant OptionGroup
            var optionsGroups = optionsGroupsXrefs.Include(o => o.RegTypesGroup).Select(o => o.RegTypesGroup);
            //var b = optionsGroups.ToList();

            //  Get all OptionsXrefs for those OptionGroups
            var optionsXrefs = optionsGroups.Include(o => o.RegTypesXrefs).SelectMany(opt => opt.RegTypesXrefs);
            //var c = optionsXrefs.ToList();

            //  Finally, get the options
            var options = optionsXrefs.Include(o => o.RegType).Select(o => o.RegType).ToList();

            if (!detached)
                return options;

            options.ForEach(o => stronglyTypedContext.Entry(o).State = EntityState.Detached);

            return options;
        }

    }
}