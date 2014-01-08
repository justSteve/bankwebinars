using BrockAllen.MembershipReboot.Ef;
using System.Collections.Generic;
using System.Data.Entity;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Repository
{
    //public class DbContextWebUserRepository<Ctx> : DbContextRepository<WebUser>, IWebUserRepository
    //    where Ctx : DbContext, new()
    //{

    //    public DbContextWebUserRepository()
    //        : this(new Ctx())
    //    {

    //    }

    //    public DbContextWebUserRepository(Ctx ctx)
    //        : base(ctx)
    //    {

    //    }

    //    public void UpdateAddresses(Address address)
    //    {
    //        var entry = db.Entry(address);

    //        if (entry.State == EntityState.Detached)
    //        {
    //            ((TTSWebinarsContext)db).Addresses.Attach(address);
    //            entry.State = EntityState.Modified;
    //        }

    //        db.SaveChanges();
    //    }
    //}
}
