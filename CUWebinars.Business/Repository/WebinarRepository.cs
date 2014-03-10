using System.Data.Entity;
using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class WebinarRepository : TTSWebinarsRepository<TTSWebinarsContext, Webinar>,  IWebinarRepository
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
                .Include(w => w.OptionsGroupsXrefs)
                .Include(w => w.Presenter)
                .Include(w => w.WebinarFiles)
                .Include(w => w.WebinarTopicXrefs)
                .Where(w => w.idWebinar == id);

            return webinar.FirstOrDefault();
        }

        public Webinar FindByIdAndDetach(int id)
        {
            var webinar = FindById(id);
            db.Entry(webinar).State = EntityState.Detached;
            return webinar;
        }

        public IQueryable<Webinar> GetUpcoming()
        {
            return items.Where(w => w.Status == WebinarStatus.Scheduled)
                    .OrderByDescending(w => w.Date);
        }

        public IQueryable<Webinar> GetAllActive()
        {
            return items.Where(w => w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Recorded);
        }


        public IQueryable<Webinar> GetRecorded()
        {
            return items.Where(w => w.Status == WebinarStatus.Recorded)
                .OrderByDescending(w => w.Date);
        }

        public IQueryable<Order> GetOrdersByWebinar(int webinarId)
        {
            return ((TTSWebinarsContext)db).Orders.Where(o => o.OrderRows.Single().Webinar.idWebinar == webinarId
                    && (o.OrderRows.Single().Status == OrderRowStatus.Billed
                        || o.OrderRows.Single().Status == OrderRowStatus.Paid
                        || o.OrderRows.Single().Status == OrderRowStatus.Submitted));
        }

        public IQueryable<Webinar> GetByTopic(int topicId)
        {
            var webinars = items.Where(w => w.WebinarTopicXrefs
                .Any(t => t.idTopic == topicId)
                    && (w.Status == WebinarStatus.Recorded || w.Status == WebinarStatus.Scheduled));
            //Logger.Debug("TopicId=" + topicId); 
            return webinars;
        }

        /// <summary>
        /// The act of getting the correct set of options to display is complex and spelled out
        /// </summary>
        /// <param name="idWebinar"></param>
        /// <returns>List of Type Option</returns>
        public List<Option> GetCurrentOptions(int idWebinar)
        {
            //resolves: Impediment 6:Implement GetCurrentOptions Method
            var paramWebinarID = new SqlParameter("idWebinar", SqlDbType.Int) { Value = idWebinar };
            List<Option> myOptions = ((TTSWebinarsContext)db).Options.SqlQuery(
                "dbo.GetWebinarsOptions @idWebinar", paramWebinarID).ToList();

            return myOptions;
        }

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
                if (((TTSWebinarsContext)db).Options.Find((int)row.RegistrationType).ShowLiveNotifications == "No"
                    && row.Status == OrderRowStatus.Abandoned
                    || row.Status == OrderRowStatus.InProcess
                    || row.Status == OrderRowStatus.Canceled)
                {
                    continue;
                }
                orders2Send.Add(order);
            }
            return orders2Send;
            //return (from order in orders let row = order.OrderRows.SingleOrDefault()
            //        where row != null 
            //        where (((TTSWebinarsContext)db).Options.Find((int) row.RegistrationType).ShowLiveNotifications != "No" 
            //        || row.Status != OrderRowStatus.Abandoned) 
            //        && row.Status != OrderRowStatus.InProcess 
            //        && row.Status != OrderRowStatus.Canceled select order).ToList();
        }
    }
}