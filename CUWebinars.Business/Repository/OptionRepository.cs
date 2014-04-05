using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class RegTypeRepository : TTSWebinarsRepository<TTSWebinarsContext, RegType>, IRegTypeRepository
    {
        public RegType FindRegType(int id)
        {
            return items.FirstOrDefault(o => o.idRegType == id);
        }

        //public IList<RegType> FindRegTypesByWebinarId(int id, bool detached)
        //{
        //    var stronglyTypedContext = (TTSWebinarsContext) db;

        //    var webinars = stronglyTypedContext.Webinars
        //        //.Include(w => w.OptionsGroupsXrefs)
        //        .Where(w => w.idWebinar == id);

        //    // There can be only one OptionsGroupsXrefs per webinar at any one time
        //    var optionsGroupsXrefs = webinars.SelectMany(w => w.OptionsGroupsXrefs);
        //    //var a = optionsGroupsXrefs.ToList();

        //    //  For each of those OptionsGroupsXrefs, get the relevant OptionGroup
        //    var optionsGroups = optionsGroupsXrefs.Include(o => o.OptionsGroup).Select(o => o.OptionsGroup);
        //    //var b = optionsGroups.ToList();

        //    //  Get all OptionsXrefs for those OptionGroups
        //    var optionsXrefs = optionsGroups.Include(o => o.OptionsXrefs).SelectMany(opt => opt.OptionsXrefs);
        //    //var c = optionsXrefs.ToList();

        //    //  Finally, get the options
        //    var options = optionsXrefs.Include(o => o.RegType).Select(o => o.RegType).ToList();

        //    if (!detached)
        //        return options;

        //    options.ForEach(o => stronglyTypedContext.Entry(o).State = EntityState.Detached);

        //    return options;
        //}

    }
}