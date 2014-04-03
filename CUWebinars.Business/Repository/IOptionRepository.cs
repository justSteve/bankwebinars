using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public interface IOptionRepository
    {
        Option FindOption(int idOption);
        IList<Option> FindOptionsByWebinarId(int id, bool detached);
    }
}
