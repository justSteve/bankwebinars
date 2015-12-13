using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class DiscountRepository : TTSWebinarsRepository<TTSWebinarsContext, Discount>, IDiscountRepository
    {
        public DiscountRepository(TTSWebinarsContext ctx)
            : base(ctx)
        {

        }

        public Discount FindDiscount(int id)
        {
            var i = items.FirstOrDefault(o => o.idDiscount == id);
            return i;
        }

        public IList<Discount> FindDiscountOption(int optionId)
        {
            return items.Where(o => o.idDiscount == optionId).ToList();
        }
        public void SaveChanges(Discount discount)
        {
            CheckDisposed();

            var entry = db.Entry(discount);
            if (entry.State == EntityState.Detached)
            {
                items.Attach(discount);
                entry.State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        

    }
}