using System.Linq;
using TTSDataLayer.Models;

namespace CUWebinars.Web.Data.Repositories.Interfaces
{
    public interface IWebUserRepository
    {
        IQueryable<WebUser> GetUsers();
    }
}
