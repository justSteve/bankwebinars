using CUWebinars.Business.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Core;

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
            var stronglyTypedContext = (TTSWebinarsContext)db;

            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            IList<RegType> regTypes = new List<RegType>();

            var regTypeIds = dataOperations.FindAllPossibleRegTypesByWebinarId(id);

            foreach (var idRegType in regTypeIds)
            {
                regTypes.Add(FindRegType(idRegType));
            }


            var regTypes_ =
                regTypes
                    .Distinct()
                    .ToList()
                    .ToDictionary(r => r, rt => rt.ShowShippedNotifications.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase), new RegTypeComparer());

            if (!detached)
                return regTypes_;

            foreach (var regType in regTypes_)
            {
                stronglyTypedContext.Entry(regType.Key).State = EntityState.Detached;
            }

            return regTypes_;

        }

        public IDictionary<RegType, bool> FindRegTypesAvailableToExistingOrder(int id, bool detached, Order order)
        {
            var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            var oRegType = row.RegistrationType;
            
            var stronglyTypedContext = (TTSWebinarsContext)db;

            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            IList<RegType> regTypes = new List<RegType>();


            var regTypeIds = dataOperations.FindAllPossibleRegTypesByWebinarId(id);

            foreach (var idRegType in regTypeIds)
            {
                regTypes.Add(FindRegType(idRegType));
            }


            var regTypes_ =
                regTypes.Where(r => r.idRegType != row.idRegType)
                    .Distinct()
                    .ToList()
                    .ToDictionary(r => r,
                        rt => rt.ShowShippedNotifications.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase),
                        new RegTypeComparer());
            bool webinarIsPast = dataOperations.WebinarIsPast(id);
            if (webinarIsPast)
            {
                if (oRegType.ShowLiveNotifications.ToLower() == "yes")
                {
                    regTypes_ =
                        regTypes.Where(r => r.ShowLiveNotifications.ToLower() == "yes" && r.idRegType != row.idRegType
                        //&& r.ShowRecordingNotifications.ToLower() == "yes"
                        )
                            .Distinct()
                            .ToList()
                            .ToDictionary(r => r,
                                rt => rt.ShowShippedNotifications.Trim()
                                    .Equals("Yes", StringComparison.OrdinalIgnoreCase),
                                new RegTypeComparer());
                    //if ()
                }
                else
                {
                    regTypes_ =
                        regTypes.Where(r => r.ShowLiveNotifications.ToLower() != "yes" && r.idRegType != row.idRegType 
                        //&& r.ShowRecordingNotifications.ToLower() == "yes"
                        )
                            .Distinct()
                            .ToList()
                            .ToDictionary(r => r,
                                rt => rt.ShowShippedNotifications.Trim()
                                    .Equals("Yes", StringComparison.OrdinalIgnoreCase),
                                new RegTypeComparer());
                }
            }
            if (!detached)
                return regTypes_;

            foreach (var regType in regTypes_)
            {
                stronglyTypedContext.Entry(regType.Key).State = EntityState.Detached;
            }

            return regTypes_;
        }
        public IDictionary<RegType, bool> FindRegTypesByWebinarId(int id, bool detached)
        {
            //IDictionary<RegType, bool> regTypesAndShippingRequirement = new Dictionary<RegType, bool>();

            var stronglyTypedContext = (TTSWebinarsContext)db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == id);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time
            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

            //when pre-event
            //  Foreach of those RegTypesGroupsXrefs, get the relevant OptionGroup
            var regtypesGroups = regTypesGroupsXrefs.Include(o => o.RegTypesGroup).Select(o => o.RegTypesGroup);

            //  Get all OptionsXrefs for those RegTypesGroups
            var regtypesXrefs = regtypesGroups.Include(o => o.RegTypesXrefs).SelectMany(opt => opt.RegTypesXrefs);

            //  Finally, get the RegTypes
            var regTypes =
                regtypesXrefs.Include(o => o.RegType)
                    .Select(o => o.RegType)
                    .Distinct()
                    .OrderBy(o => o.SortOrder)
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

        public RegType GetRegTypeByLabel(string regType, int idWebinar)
        {
            var stronglyTypedContext = (TTSWebinarsContext)db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == idWebinar);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time
            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

            //when pre-event
            //  For each of those RegTypesGroupsXrefs, get the relevant OptionGroup
            var regtypesGroups = regTypesGroupsXrefs.Include(o => o.RegTypesGroup).Select(o => o.RegTypesGroup);

            //  Get all OptionsXrefs for those RegTypesGroups
            var regtypesXrefs = regtypesGroups.Include(o => o.RegTypesXrefs).SelectMany(opt => opt.RegTypesXrefs);

            //  Finally, get the RegTypes
            var regTypeFound =
                regtypesXrefs.Include(o => o.RegType)
                    .Select(o => o.RegType).Where(o => o.OptionLabel.StartsWith(regType)).SingleOrDefault();
            return regTypeFound;




        }

        public RegType FindRegType4ExpressPostback2(string regType, int idWebinar)
        {
            var stronglyTypedContext = (TTSWebinarsContext)db;

            var webinars = stronglyTypedContext.Webinars
                .Where(w => w.idWebinar == idWebinar);

            // There can be only one RegTypesGroupsXrefs per webinar at any one time
            var regTypesGroupsXrefs = webinars.SelectMany(w => w.RegTypesGroupsXref);

            //when pre-event
            //  For each of those RegTypesGroupsXrefs, get the relevant OptionGroup
            var regtypesGroups = regTypesGroupsXrefs.Include(o => o.RegTypesGroup).Select(o => o.RegTypesGroup);

            //  Get all OptionsXrefs for those RegTypesGroups
            var regtypesXrefs = regtypesGroups.Include(o => o.RegTypesXrefs).SelectMany(opt => opt.RegTypesXrefs);

            //  Finally, get the RegTypes
            var regTypeFound =
                regtypesXrefs.Include(o => o.RegType)
                    .Select(o => o.RegType).Where(o => o.OptionLabel.StartsWith(regType)).SingleOrDefault();
            return regTypeFound;

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