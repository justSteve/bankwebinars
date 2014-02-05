using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class OptionRepository : TTSWebinarsRepository<TTSWebinarsContext, Option>, IOptionRepository
    {
        public Option FindOption(int id)
        {
            return items.Find(id);
        }
    }
}