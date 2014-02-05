using CUWebinars.Business.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace CUWebinars.Business.Repository
{
    public class TTSWebinarsRepository<Ctx, T> : IDisposable
        where Ctx : DbContext, new()      
        where T : class
    {
        protected readonly DbContext db;
        protected DbSet<T> items;

        public TTSWebinarsRepository()
            : this(new Ctx())
        {

        }

        public TTSWebinarsRepository(Ctx ctx)
        {
            db = ctx;
            items = db.Set<T>();
        }

        public void Add(T item)
        {
            items.Add(item);
            db.SaveChanges();
        }

        public  IEnumerable<T> GetAll()
        {
            return items;
        }

        public T FindById(int id)
        {
            var item = items.Find(id);
            return item;
        }

        protected void CheckDisposed()
        {
            if (db == null)
            {
                throw new ObjectDisposedException("TTSWebinarsRepository<Ctx, T>");
            }
        }

        public void Dispose()
        {
            if (db.TryDispose())
            {
                items = null;
            }
        }
    }
}
