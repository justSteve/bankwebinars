using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IRegTypeRepository
    {
        RegType FindRegType(int idRegType);
        //IList<RegType> FindRegTypesByWebinarId(int id, bool detached);
    }
}
