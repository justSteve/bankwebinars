using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public interface IAdditionalLocationsRepository
    {
        IEnumerable<AdditionalLocation> GetAdditionalLocationsForOrderRow(int idOrderRow);
        void DeleteAdditionalLocationsByOrderRowId(int idOrderRow);
    }
}