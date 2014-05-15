using System.Data.Entity;
using System.IO;
using System.Runtime.Remoting.Contexts;
using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class WebinarRepository : TTSWebinarsRepository<TTSWebinarsContext, Webinar>, IWebinarRepository
    {
        public WebinarRepository()
        {

        }

        public WebinarRepository(TTSWebinarsContext context)
            : base(context)
        {

        }

        //public static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Webinar FindByIdLoaded(int id)
        {
            var webinar = items.Include(w => w.OrderRows)
                //.Include(w => w.OptionsGroupsXrefs)
                .Include(w => w.Presenter)
                //.Include(w => w.WebinarFiles)
                .Include(w => w.WebinarFiles.Select(wf => wf.Webinar))
                .Include(w => w.WebinarTopicXrefs.Select(wt => wt.Topic))
                .Where(w => w.idWebinar == id);

            return webinar.FirstOrDefault();
        }

        public Webinar FindByIdAndDetach(int id)
        {
            var webinar = FindById(id);
            db.Entry(webinar).State = System.Data.Entity.EntityState.Detached;
            return webinar;
        }

        public IQueryable<Webinar> GetUpcoming()
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => (w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Active || w.Status == WebinarStatus.InProgress))
                .OrderByDescending(w => w.Date);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            var webinar = items.Include(w => w.Presenter.WebUser)
                .Include(w => w.Presenter.Webinars)
                .First(w => w.idWebinar == id);

            return webinar;
        }

        public IQueryable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return ((TTSWebinarsContext)db).WebinarTopicXrefs
                //.Include(x => x.Webinar)
                //.Include(w => w.Topic)
                .Where(t => t.idWebinar == idWebinar).Select(t => t.Topic);
        }

        public IQueryable<Webinar> GetAllActive()
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Recorded || w.Status == WebinarStatus.Active || w.Status == WebinarStatus.InProgress);
        }


        public IQueryable<Webinar> GetRecorded()
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => w.Status == WebinarStatus.Recorded)
                .OrderByDescending(w => w.Date);
        }

        public IQueryable<Order> GetOrdersByWebinar(int webinarId)
        {
            return ((TTSWebinarsContext)db).Orders.Where(o => o.OrderRows.Single().Webinar.idWebinar == webinarId
                    && (o.OrderStatus == OrderStatus.Billed
                        || o.OrderStatus == OrderStatus.Paid
                        || o.OrderStatus == OrderStatus.Submitted));
        }
        public IQueryable<Order> GetAllOrdersByWebinarForUser(int webinarId, int userId)
        {
            return ((TTSWebinarsContext)db).Orders
                .Where(o => o.OrderRows.FirstOrDefault().Webinar.idWebinar == webinarId && o.idUser == userId);
        }

        public IQueryable<Webinar> GetByTopic(int topicId)
        {
            var webinars = items.Include(i => i.WebinarTopicXrefs.Select(w => w.Topic))
                .Include(i => i.Presenter.WebUser)
                .Where(w => w.WebinarTopicXrefs
                .Any(t => t.idTopic == topicId)
                    && (w.Status == WebinarStatus.Recorded || w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Active || w.Status == WebinarStatus.InProgress));
            //Logger.Debug("TopicId=" + topicId); 

            return webinars;
        }

        public List<RegType> GetCurrentOptions(int idWebinar)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The act of getting the correct set of options to display is complex and spelled out
        /// </summary>
        /// <param name="idWebinar"></param>
        /// <returns>List of Type RegType</returns>
        //public List<RegType> GetCurrentOptions(int idWebinar)
        //{
        //    //resolves: Impediment 6:Implement GetCurrentOptions Method
        //    var paramWebinarID = new SqlParameter("idWebinar", SqlDbType.Int) { Value = idWebinar };
        //    List<RegType> myOptions = ((TTSWebinarsContext)db).Options.SqlQuery(
        //        "dbo.GetWebinarsOptions @idWebinar", paramWebinarID).ToList();

        //    return myOptions;
        //}

        public IList<Order> GetOrdersByWebinarForConnectionInfo(int id)
        {
            IQueryable<Order> orders = GetOrdersByWebinar(id);
            IList<Order> orders2Send = new List<Order>();
            foreach (var order in orders)
            {
                var row = order.OrderRows.SingleOrDefault();
                if (row == null)
                {
                    //TODO: log this exception condition
                    continue;
                }
                if (((TTSWebinarsContext)db).RegTypes.Find(row.RegistrationType).ShowLiveNotifications == "No"
                    //&& Order.OrderStatus == OrderStatus.Abandoned
                    //|| row.Status == OrderStatus.InProcess
                    //|| row.Status == OrderStatus.Canceled
                    )
                {
                    continue;
                }
                orders2Send.Add(order);
            }
            return orders2Send;
            //return (from order in orders let row = order.OrderRows.SingleOrDefault()
            //        where row != null 
            //        where (((TTSWebinarsContext)db).Options.Find((int) row.RegistrationType).ShowLiveNotifications != "No" 
            //        || row.Status != OrderStatus.Abandoned) 
            //        && row.Status != OrderStatus.InProcess 
            //        && row.Status != OrderStatus.Canceled select order).ToList();
        }
    }
}