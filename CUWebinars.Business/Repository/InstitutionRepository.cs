using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Business.Repository
{
    public class InstitutionRepository : TTSWebinarsRepository<TTSWebinarsContext, Institution>, IInstitutionRepository
    {
        public InstitutionRepository()
        {

        }
    }
}
