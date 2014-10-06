using System;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IRegTypeRepository : IDisposable
    {
        RegType FindRegType(int idRegType);
        IList<RegType> FindRegTypesForOption(int optionId);
        IList<RegType> FindRegTypesByWebinarId(int id, bool detached);
    }
}
