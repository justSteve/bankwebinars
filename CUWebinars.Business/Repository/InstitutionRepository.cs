using System.Linq;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public class InstitutionRepository : TTSWebinarsRepository<TTSWebinarsContext, Institution>, IInstitutionRepository
    {
        public InstitutionRepository()
        {

        }

        public IEnumerable<Institution> GetByNameAndZipCode(string name, string zip)
        {
            return items.Where(i => i.InstitutionName == name && i.Zip == zip);
        }
    }
}
