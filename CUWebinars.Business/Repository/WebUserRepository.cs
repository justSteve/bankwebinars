using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text;
using CUWebinars.Business.Models;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Services;
using log4net.Repository.Hierarchy;
using Ninject.Extensions.Logging;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Business.Repository
{
    public class WebUserRepository : TTSWebinarsRepository<TTSWebinarsContext, WebUser>, IWebUserRepository
    {
        public RefDataRepository RefContext { get; set; }
        private readonly ILogger _logger;

        public WebUserRepository(ILogger logger)
        {
            RefContext = new RefDataRepository();
            //_logger = logger;
        }

        public WebUserRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {
            RefContext = new RefDataRepository();
            
        }
        ILogger loggerForOrderManagementService = new Log4NetLogger(typeof(OrderManagementService));

        /// <summary>
        /// Finds the Highest Id currently in use so newly created WebUser objects have an Id.
        /// Id value is not db-generated so that objects created by the seed method 
        /// can more easily import the Id used by the old system - let's us use the same Affiliate and Presenter
        /// Ids that we've grown used to using.
        /// </summary>
        /// <returns></returns>
        public int FindHighestUserId()
        {
            int nextId = RefContext.GetWebUsers()
                .OrderByDescending(i => i.idUser)
                .Select(i => i.idUser).Single();

            return nextId++;
        }

        public int? FindInstitution(string zip, string institutionName)
        {
            int myInst = RefContext.GetInstitutions()
                .Where(i => i.InstitutionName == institutionName && i.Zip == zip)
                .Select(i => i.idInstitution)
                .SingleOrDefault();

            return myInst;
        }

        public WebUser FindByIdLoaded(int id)
        {
            var webUser = items
                .Include(i => i.Addresses)
                //.Include(i => i.Affiliate)
                .Include(i => i.Institution)
                .Include(i => i.Presenter)
                .Where(i => i.idUser == id);
            return webUser.FirstOrDefault();
        }

        public WebUser GetWebUserByEmail(string email)
        {
            return items
                .Include(wu => wu.Addresses)
                .Include(wu => wu.Institution)
                .Where(w => w.email == email).SingleOrDefault();
        }

        public IEnumerable<WebUser> GetWebUsersByLastName(string lastName)
        {
            return items.Where(webUser => webUser.LastName.ToLower().Contains(lastName))
                .OrderBy(webUser => webUser.LastName)
                .ThenBy(webUser => webUser.FirstName);
        }

        public WebUser GetWebUserByEmailLoadedWithOrdersData(string email)
        {
            return items
                .Include(wu => wu.Addresses)
                .Include(wu => wu.Institution)
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.Discount)))
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.AdditionalLocation)))
                .Include(wu => wu.Orders.Select(o => o.OrderRows.Select(or => or.RegistrationType)))
                .Where(wu => wu.email == email).SingleOrDefault();
        }

        public void UpdateAddresses(Address address)
        {
            var entry = db.Entry(address);

            if (entry.State == EntityState.Detached)
            {
                ((TTSWebinarsContext)db).Addresses.Attach(address);
                entry.State = EntityState.Modified;
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    var errorMsg = new StringBuilder();

                    errorMsg.Append(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errorMsg.Append(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                    loggerForOrderManagementService.Error(errorMsg.ToString());
                }
                throw;
            }
        }

        public IEnumerable<WebUser> GetWebusersForWebinarWithRegtypes(int idWebinar, IEnumerable<int> regTypeIds)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => regTypeIds.Contains(or.idRegType))
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }
        public IEnumerable<WebUser> GetWebusersForRecordingPostedNotifications(int idWebinar)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowRecordingNotifications == "Yes")
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }
        public IEnumerable<WebUser> GetWebusersForLiveNotifications(int idWebinar)
        {
            var webUsers = ((TTSWebinarsContext)db).OrderRows
                .Include(or => or.Order)
                .Where(or => or.idWebinar == idWebinar)
                .Where(or => or.RegistrationType.ShowLiveNotifications == "Yes")
                .Select(o => o.Order)
                .Select(o => o.WebUser);

            return webUsers;
        }

        public void Update(WebUser webUser)
        {
            CheckDisposed();

            var entry = db.Entry(webUser);
            if (entry.State == EntityState.Detached)
            {
                items.Attach(webUser);
                entry.State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        public DbContext DbContext
        {
            get { return db; }
        }
    }
}
