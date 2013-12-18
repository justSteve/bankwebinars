using BrockAllen.MembershipReboot.Ef;
using System.Data.Entity;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    public class DbContextInstitutionRepository<Ctx> : DbContextRepository<Institution>, IInstitutionRepository
        where Ctx : DbContext, new()

    {
        public DbContextInstitutionRepository()
            : this(new Ctx())
        {

        }

        public DbContextInstitutionRepository(Ctx ctx)
            : base(ctx)
        {

        }
    }
}
