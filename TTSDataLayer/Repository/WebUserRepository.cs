using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class WebUserRepository : DbContextWebUserRepository<TTSWebinarsContext>
    {
        public RefDataRepository RefContext { get; set; }

        public WebUserRepository()
        {
            RefContext = new RefDataRepository();
        }

        public WebUserRepository(string name)
            : base(new TTSWebinarsContext(name))
        {

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
    }
}
