using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IRegTypeRepository : IDisposable
    {
        RegType FindRegType(int idRegType);
        IList<RegType> FindRegTypeOption(int optionId);
        IDictionary<RegType, bool> FindRegTypesByWebinarId(int id, bool detached);
        IDictionary<RegType, bool> FindAllPossibleRegTypesByWebinarId(int id, bool detached);
        bool IsShippingAddressRequired(int regTypeId);

        RegType GetRegTypeByLabel(string regType, int idWebinar);
        RegType FindRegType4ExpressPostback2(string idRegType, int q18QWebinarid18);
        IDictionary<RegType, bool> FindRegTypesAvailableToExistingOrder(int id, bool b, Order order);
    }
}
