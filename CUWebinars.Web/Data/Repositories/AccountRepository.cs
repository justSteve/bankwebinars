using System.Linq;
using CUWebinars.Web.Data.Repositories.Interfaces;
using log4net;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        TTSWebinarsContext _ctx;
        public static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AccountRepository(TTSWebinarsContext ctx)
        {
            _ctx = ctx;
        }
        public IQueryable<OrderRow> GetOrdersByUser(int userId)
        {
            return _ctx.OrderRow.Where(w => w.idWebinar == 801);

        }
    }
}