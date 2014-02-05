using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IOptionRepository
    {
        Option FindOption(int id);
    }
}
