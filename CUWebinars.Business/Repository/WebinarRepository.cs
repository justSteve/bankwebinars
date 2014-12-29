using System;
using System.Data;
using System.Data.SqlClient;
using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Data.Entity;
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

        public void Delete(Webinar webinar)
        {
            items.Remove(webinar);
            db.SaveChanges();
        }//

        public Webinar FindByIdLoaded(int id)
        {
            var webinar = items.Include(w => w.OrderRows)
                .Include(w => w.RegTypesGroupsXref)
                .Include(w => w.Presenter.WebUser)
                .Include(w => w.WebinarFiles)
                .Include(w => w.WebinarTopicXrefs.Select(wt => wt.Topic))
                .Where(w => w.idWebinar == id);

            return webinar.FirstOrDefault();
        }

        public IQueryable<Webinar> GetUpcoming()
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(
                    w =>
                        (w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Active ||
                         w.Status == WebinarStatus.InProgress))
                .OrderByDescending(w => w.Date);
        }

        public Webinar GetWebinarByIdIncludingAllWebinarsByPresenter(int id)
        {
            return items.Include(w => w.Presenter.WebUser)
                .Include(w => w.Presenter.Webinars)
                .First(w => w.idWebinar == id);

            //return webinar;
        }

        public IQueryable<Topic> GetTopicsPerWebinar(int idWebinar)
        {
            return ((TTSWebinarsContext)db).WebinarTopicXrefs
                //.Include(x => x.Webinar)
                //.Include(w => w.Topic)
                .Where(t => t.idWebinar == idWebinar).Select(t => t.Topic);
        }
        public IQueryable<WebinarFile> GetWebinarFilesPerWebinar(int idWebinar)
        {
            return ((TTSWebinarsContext)db).WebinarFiles
                .Where(t => t.idWebinar == idWebinar);
        }

        public int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar)
        {

            using (var sqlConnection = new SqlConnection("Data Source=tcp:kmow9uloz6.database.windows.net,1433;Initial Catalog=BankWebinars;User Id=TTSOp@kmow9uloz6;Password=EntRisR..E121114;"))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        " SELECT  regType.idRegtype FROM    dbo.RegType regType INNER JOIN dbo.RegTypesXref rx " +
                        "ON rx.idRegType = regType.idRegType INNER JOIN dbo.RegTypesGroups rtg " +
                        "ON rtg.idRegTypeGroup = rx.idRegTypeGroup INNER JOIN dbo.RegTypesGroupsXref rtgX " +
                        "ON rtgX.idRegTypeGroup = rtg.idRegTypeGroup WHERE   rtgX.idWebinar = " +
                        idWebinar + " AND regType.RegTypeLabel = '" + registrationType + "'";
                    
                    command.CommandType = CommandType.Text;


                    //SqlParameter parameterRT = new SqlParameter();
                    //parameterRT.ParameterName = "@registrationType";
                    //parameterRT.SqlDbType = SqlDbType.;
                    //parameterRT.Direction = ParameterDirection.Input;
                    //parameterRT.Value = registrationType;

                    //SqlParameter parameteridWebinar = new SqlParameter
                    //{
                    //    ParameterName = "@idWebinar",
                    //    SqlDbType = SqlDbType.Int,
                    //    Direction = ParameterDirection.Input,
                    //    Value = idWebinar
                    //};

                    //// Add the parameter to the Parameters collection. 
                    //command.Parameters.Add(parameterRT);
                    //command.Parameters.Add(parameteridWebinar);

                    var message = command.ExecuteScalar();
                    return Convert.ToInt32(message);
                }
            }

        }

        public IQueryable<Webinar> GetAllActive()
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(
                    w =>
                        w.Status == WebinarStatus.Scheduled || w.Status == WebinarStatus.Recorded ||
                        w.Status == WebinarStatus.Active || w.Status == WebinarStatus.InProgress);
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
            return ((TTSWebinarsContext)db).Orders.Where(o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Webinar.idWebinar == webinarId
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
                            &&
                            (w.Status == WebinarStatus.Recorded || w.Status == WebinarStatus.Scheduled ||
                             w.Status == WebinarStatus.Active || w.Status == WebinarStatus.InProgress));
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

        public void Update(Webinar webinar)
        {
            db.Entry(webinar).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}