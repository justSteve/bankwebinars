using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IRegTypeRepository
    {
        RegType FindRegType(RegType idRegType);
        IList<RegType> FindRegTypesByWebinarId(int id, bool detached);
    }
}
