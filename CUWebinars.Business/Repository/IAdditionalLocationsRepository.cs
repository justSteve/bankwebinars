using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IAdditionalLocationsRepository
    {
        void DeleteAdditionalLocationsByOrderRowId(int idOrderRow);
    }
}