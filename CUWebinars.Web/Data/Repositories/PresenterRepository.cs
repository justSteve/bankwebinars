using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories
{
    public class PresenterRepository: IPresenterRepository

    {
        public TTSWebinarsContext _ctx { get; set; }

        public PresenterRepository(TTSWebinarsContext ctx)
        {
            _ctx= ctx;
        }

        public IQueryable<Webinar> GetWebinars(int presenterId)
        {
            //var presenters = _ctx.Presenters;
            //var webinars = _ctx.Webinars;

            var query = from webinar in _ctx.Webinars
                        where webinar.Presenter.idUser == presenterId
            select webinar;


            var thisList = query;
            return thisList;

        }
    }
}