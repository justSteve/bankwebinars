using CUWebinars.Business.Models;
using System.Linq;

namespace CUWebinars.Business.Repository
{
    public class AdditionalLocationRepository : TTSWebinarsRepository<TTSWebinarsContext, AdditionalLocation>, IAdditionalLocationsRepository
    {
        public AdditionalLocationRepository()
        {
            
        }

        public AdditionalLocationRepository(TTSWebinarsContext context)
            : base(context)
        {

        }


        public void DeleteAdditionalLocationsByOrderRowId(int idOrderRow)
        {
            var additionalLocations = items.Where(al => al.idOrderRow == idOrderRow);

            items.RemoveRange(additionalLocations);

            db.SaveChanges();
        }
    }
}
