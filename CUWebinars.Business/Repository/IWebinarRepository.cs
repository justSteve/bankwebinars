using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IWebinarRepository
    {
        Webinar FindById(int id);
        Webinar FindByIdLoaded(int id);
        IQueryable<Webinar> GetUpcoming();
        IQueryable<Webinar> GetRecorded();
        IQueryable<Webinar> GetAllActive();
        IQueryable<Order> GetOrdersByWebinar(int webinarId);
        IQueryable<Order> GetAllOrdersByWebinarForUser(int webinarId, int userId);
        IQueryable<Webinar> GetByTopic(int topicId);
        List<RegType> GetCurrentOptions(int idWebinar);
        IList<Order> GetOrdersByWebinarForConnectionInfo(int id);
        Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id);
        IQueryable<Topic>  GetTopicsPerWebinar(int idWebinar);
    }
}
