using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using log4net;


namespace CUWebinars.Web.Data.Repositories
{
    public class WebinarRepository : IWebinarRepository
    {
        TTSWebinarsContext _ctx;
        public static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WebinarRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }
        public IQueryable<Webinar> GetUpcoming()
        {
            return _ctx.Webinars.Where(w => w.Status == WebinarStatus.Scheduled);
        }

        public IQueryable<Webinar> GetAllActive()
        {
            return _ctx.Webinars.Where(w => w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Recorded);
        }


        public IQueryable<Webinar> GetRecorded()
        {
            return _ctx.Webinars.Where(w => w.Status == WebinarStatus.Recorded);
        }

        public IQueryable<Order> GetOrdersByWebinar(int webinarId)
        {
            return _ctx.Orders.Where(o => o.OrderRows.Single().Webinar.idWebinar == webinarId
                    && (o.OrderRows.Single().Status == OrderRowStatus.Billed
                        || o.OrderRows.Single().Status == OrderRowStatus.Paid
                        || o.OrderRows.Single().Status == OrderRowStatus.Submitted));
        }

        public IQueryable<Webinar> GetByTopic(int topicId)
        {

            var webinars = _ctx.Webinars.Where(w => w.WebinarTopicXrefs
                .Any(t => t.idTopic == topicId)
                    && (w.Status == WebinarStatus.Recorded || w.Status == WebinarStatus.Scheduled));
            Logger.Debug("TopicId=" + topicId);
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
            var paramWebinarID = new SqlParameter("idWebinar", SqlDbType.Int) {Value = idWebinar};
            List<Option> myOptions  = _ctx.Options.SqlQuery(
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
                if (_ctx.Options.Find((int)row.RegistrationType).ShowLiveNotifications == "No"
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
            //        where (_ctx.Options.Find((int) row.RegistrationType).ShowLiveNotifications != "No" 
            //        || row.Status != OrderRowStatus.Abandoned) 
            //        && row.Status != OrderRowStatus.InProcess 
            //        && row.Status != OrderRowStatus.Canceled select order).ToList();
        }
    }
}