using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories.Interfaces
{
    public interface IWebinarRepository
    {
        IQueryable<Webinar> GetUpcoming();
        IQueryable<Webinar> GetRecorded();
        IQueryable<Webinar> GetAllActive();
        IQueryable<Order> GetOrdersByWebinar(int webinarId);
        IQueryable<Webinar> GetByTopic(int topicId);
        List<Option> GetCurrentOptions(int idWebinar);
        IList<Order> GetOrdersByWebinarForConnectionInfo(int id);

    }
}
