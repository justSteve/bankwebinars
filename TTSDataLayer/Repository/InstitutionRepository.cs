using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class InstitutionRepository : DbContextInstitutionRepository<TTSWebinarsContext>
    {
        public InstitutionRepository()
        {

        }

        public InstitutionRepository(string name)
            : base(new TTSWebinarsContext(name))
        {
        }

        
    }
}
