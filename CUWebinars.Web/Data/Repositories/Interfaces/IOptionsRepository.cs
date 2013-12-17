using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories.Interfaces
{
    public interface IOptionsRepository
    {
        IQueryable<Option> GetOptionsByWebinar(int webinarId);
    }
}
