using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IWebinarRepository
    {
        Webinar FindById(int id);
        Webinar FindByIdLoaded(int id);
        Webinar FindByIdAndDetach(int id);
        IQueryable<Webinar> GetUpcoming();
        IQueryable<Webinar> GetRecorded();
        IQueryable<Webinar> GetAllActive();
        IQueryable<Order> GetOrdersByWebinar(int webinarId);
        IQueryable<Webinar> GetByTopic(int topicId);
        List<Option> GetCurrentOptions(int idWebinar);
        IList<Order> GetOrdersByWebinarForConnectionInfo(int id);

        IQueryable<Topic>  GetTopicsPerWebinar(int idWebinar);
    }
}
