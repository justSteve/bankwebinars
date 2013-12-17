using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories.Interfaces
{
    public interface IPresenterRepository
    {
        IQueryable<Webinar> GetWebinars(int presenterId);
    }
}
