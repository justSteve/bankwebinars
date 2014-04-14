using CUWebinars.Business.Models;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class WebUserRepository : TTSWebinarsRepository<TTSWebinarsContext, WebUser>, IWebUserRepository
    {
        public RefDataRepository RefContext { get; set; }

        public WebUserRepository()
        {
            RefContext = new RefDataRepository();
        }

        public WebUserRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {
            RefContext = new RefDataRepository();
        }

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
                .Include(i => i.Affiliate)
                .Include(i => i.Institution)
                .Include(i => i.Presenter)
                .Where(i => i.idUser == id);
            return webUser.FirstOrDefault();
        }

        public WebUser GetWebUserByEmail(string email)
        {
            return items
                .Include("Addresses")
                .Include("Institution")
                .Where(w => w.email == email).SingleOrDefault();
        }

        public void UpdateAddresses(Address address)
        {
            var entry = db.Entry(address);

            if (entry.State == EntityState.Detached)
            {
                ((TTSWebinarsContext)db).Addresses.Attach(address);
                entry.State = EntityState.Modified;
            }

            db.SaveChanges();
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
