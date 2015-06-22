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
        
    }
}
