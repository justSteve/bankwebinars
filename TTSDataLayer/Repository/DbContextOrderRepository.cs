using BrockAllen.MembershipReboot.Ef;
using System.Data.Entity;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class DbContextOrderRepository<Ctx> : DbContextRepository<Order>, IOrderRepository
        where Ctx : DbContext, new()
    {

        public DbContextOrderRepository()
            : this(new Ctx())
        {

        }

        public DbContextOrderRepository(Ctx ctx)
            : base(ctx)
        {

        }
    }
}
