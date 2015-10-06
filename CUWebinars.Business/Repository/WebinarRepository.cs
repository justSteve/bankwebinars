using System;
using System.Configuration;
using System.Diagnostics;
using CUWebinars.Business.Core;
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

        public void AddAdditionalLocationsLookupPrice(AdditionalLocationsLookupPrice additionalLocationsLookupPrice)
        {
            ((TTSWebinarsContext)db).AdditionalLocationsLookupPrices.Add(additionalLocationsLookupPrice);
        }

        public void Delete(Webinar webinar)
        {
            items.Remove(webinar);
            db.SaveChanges();
        }

        public void MarkForDeletion(object domainObject)
        {
            db.Entry(domainObject).State = EntityState.Deleted;
        }

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

        public IEnumerable<Webinar> FindByPresenterLastName(string lastName)
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => w.Presenter.WebUser.LastName.ToLower().Contains(lastName.ToLower())
                    && w.Status != WebinarStatus.Archived
                    && w.Status != WebinarStatus.Pending
                    && w.Status != WebinarStatus.Deleted
                    );
        }

        public IEnumerable<Webinar> FindByPresenterFullName(string searchTerm)
        {
            return items.Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                    .Include(w => w.Presenter.WebUser)
                    .Where(w => w.Presenter.WebUser.FirstName.ToLower().StartsWith(searchTerm.ToLower())
                        && w.Presenter.WebUser.LastName.ToLower().EndsWith(searchTerm.ToLower())
                        && w.Status != WebinarStatus.Archived
                        && w.Status != WebinarStatus.Pending
                        && w.Status != WebinarStatus.Deleted
        );
        }

        public IEnumerable<Webinar> FindByDescription(string searchTerm)
        {
            var stronglyTypedContext = (TTSWebinarsContext)db;

            // 1st get all topics with the topicDescription
            var topicsOfSearch = stronglyTypedContext.Topics
                .Include(t => t.WebinarTopicXrefs.Select(wtx => wtx.Webinar))
                .Where(t => t.topicDesc.ToLower().Contains(searchTerm.ToLower())
                );

            // project that into a list of webinars
            var searchTopics = topicsOfSearch.SelectMany(t => t.WebinarTopicXrefs)
                .Select(w => w.Webinar)
                .Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => w.Status != WebinarStatus.Archived
                    && w.Status != WebinarStatus.Deleted
                    && w.Status != WebinarStatus.Pending
                    );


            var searchDesc = stronglyTypedContext.Webinars
                .Include(w => w.WebinarTopicXrefs.Select(wtx => wtx.Topic))
                .Include(w => w.Presenter.WebUser)
                .Where(w => w.DescriptionLong.ToLower().Contains(searchTerm)
                        || w.Title.ToLower().Contains(searchTerm)
                    || w.WhoAttend.ToLower().Contains(searchTerm)
                    || w.LearnBody.ToLower().Contains(searchTerm)
                    && w.Status != WebinarStatus.Archived
                    && w.Status != WebinarStatus.Deleted
                    && w.Status != WebinarStatus.Pending

                    );

            var result = searchTopics.Union(searchDesc);
            return result;
        }

        public IQueryable<AdditionalLocationsLookupPrice> GetAdditionalLocationsLookupPricesForWebinar(int idWebinar)
        {
            return ((TTSWebinarsContext)db).AdditionalLocationsLookupPrices.Where(a => a.idWebinar == idWebinar);
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

        public IQueryable<RegTypesGroup> GetRegTypeGroupsForWebinars(int idWebinar)
        {
            return items.Where(w => w.idWebinar == idWebinar)
                .SelectMany(webinar => webinar.RegTypesGroupsXref)
                .Select(r => r.RegTypesGroup).Where(r => r.RegTypeGroupDesc.ToLower().Contains("15") && !r.RegTypeGroupDesc.ToLower().Contains("post"));
        }

        public RegTypesGroupsXref GetRegTypesGroupsXref(int idRegTypesGroupsXref, int idWebinar)
        {
            return ((TTSWebinarsContext)db).RegTypesGroupsXrefs.SingleOrDefault(r => r.idRegTypeGroup == idRegTypesGroupsXref && r.idWebinar == idWebinar);
        }
        public WebinarTopicXref GetWebinarTopicXref(int idTopic, int idWebinar)
        {
            return ((TTSWebinarsContext)db).WebinarTopicXrefs.SingleOrDefault(r => r.idTopic == idTopic && r.idWebinar == idWebinar);
        }

        public IQueryable<RegTypesGroup> GetUpcomingRegTypesForWebinars()
        {
            return ((TTSWebinarsContext)db).RegTypesGroups.Where(
                    r => r.RegTypeGroupDesc.ToLower().Contains("15") && !r.RegTypeGroupDesc.ToLower().Contains("post")
                    );
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
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.GetRegTypeByLableAndWebinar(registrationType, idWebinar);

        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public void SynchToLegacy()
        {
            var dataOperations = new MigrationOperations(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString, ConfigurationManager.ConnectionStrings["LegacyConnection"].ConnectionString);

            dataOperations.CopyLegacyWebinars();

        }

        public int GetRegTypeByACS(string registrationType, int idWebinar)
        {
            //first step is to convert ACS lables to TTS version
            var dataOperations = new DataOperations(ConfigurationManager.ConnectionStrings["LoggerConnection"].ConnectionString);

            var retVal = GetRegTypeByLableAndWebinar(dataOperations.FindRegTypeForACS(registrationType), idWebinar);
            return retVal;
        }

        public Double[] GetCostOfUpgrades(int idRegType)
        {
            var dataOperations = new DataOperations(TtsConfig.DefaultConnectionString);

            return dataOperations.GetCostOfUpgrades(idRegType);

        }

        public Webinar GetWebinarByJoinCode(string joinCode)
        {
            var orderRow = ((TTSWebinarsContext)db).OrderRows.SingleOrDefault(or => or.TtsJoinUrl == joinCode);

            if (!ReferenceEquals(null, orderRow))
            {
                return items.Include(w => w.Presenter.WebUser)
                    .Include(w => w.OrderRows)
                    .SingleOrDefault(w => w.idWebinar == orderRow.idWebinar);
            }

            return null;
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

        public IQueryable<Topic> GetAllTopics()
        {
            return ((TTSWebinarsContext)db).Topics;
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
            return ((TTSWebinarsContext) db).Orders
                .Where(
                    o =>
                        o.OrderRows.FirstOrDefault(or => or.RowStatus == OrderRowStatus.Active).Webinar.idWebinar ==
                        webinarId
                        && (o.OrderStatus == OrderStatus.Billed
                            || o.OrderStatus == OrderStatus.Paid
                            || o.OrderStatus == OrderStatus.Submitted))
                .Include(o => o.Affiliate)
                .Include(o => o.WebUser)
                .Include(o => o.OrderRows);

            //.Include(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType);
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
                var row = order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active);
                if (row == null)
                {

                    continue;
                }
                if (((TTSWebinarsContext)db).RegTypes.Find(row.RegistrationType)
                    .ShowLiveNotifications == "No")
                {
                    continue;
                }
                if (order.OrderStatus != OrderStatus.Abandoned
                    && order.OrderStatus != OrderStatus.AwaitingVerification
                    && order.OrderStatus != OrderStatus.Canceled
                    && order.OrderStatus != OrderStatus.InProcess
                    && order.OrderStatus != OrderStatus.Unknown)
                {
                    orders2Send.Add(order);
                }
            }
            return orders2Send;
        }

        public void Update(Webinar webinar)
        {
            db.Entry(webinar).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}