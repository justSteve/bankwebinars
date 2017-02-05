using System.Linq;
using System.Linq.Expressions;
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

            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
        }

        public void Remove(T item)
        {
            items.Remove(item);
            db.SaveChanges();
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

        public IEnumerable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            return includeProperties.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(items, (current, includeProperty) => current.Include(includeProperty));
        }

        public T FindById(int id)
       {
            var item = items.Find(id);
            return item;
        }

        public T FindByIdAndDetachItem(int id)
        {
            var item = items.Find(id);
            db.Entry(item).State = EntityState.Detached;
            return item;
        }
        public T AttachItem(T item)
        {
            items.Attach(item);
            return item;
        }

        public bool Exists(T item)
        {
            return items.Local.Any(e => e == item);
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
