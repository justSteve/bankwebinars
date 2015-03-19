using System.Linq;
using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public class InstitutionRepository : TTSWebinarsRepository<TTSWebinarsContext, Institution>, IInstitutionRepository
    {
        public InstitutionRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {

        }

        public int FindFirst()
        {
            return items.First().idInstitution;
        }

        public Institution GetByDomain(string domain)
        {
            return items.FirstOrDefault(i => i.domainName.ToLower().Contains(domain.ToLower()));
        }

        public IEnumerable<Institution> GetByNameAndZipCode(string name, string zip)
        {
            return items.Where(i => i.InstitutionName == name && i.Zip == zip);
        }

        public IEnumerable<Institution> GetInstitutionsByName(string name)
        {
            return items.Where(i => i.InstitutionName.ToLower().Contains(name.ToLower()));
        }
    }
}
