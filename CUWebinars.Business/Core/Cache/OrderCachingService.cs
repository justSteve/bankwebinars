using CUWebinars.Business.Core.Cache;
using System;
using System.Runtime.Caching;

namespace CUWebinars.Web.Core.Cache
{
    public class OrderCachingService : ICachingService
    {
        public ObjectCache ObjectCache { get { return MemoryCache.Default; }}
        
        public object Add(string key, object entry, DateTime expirey)
        {
            var cachedEntry = ObjectCache[key];

            if (cachedEntry != null)
                return cachedEntry;

            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = expirey.AddHours(1);


            ObjectCache.Add(new CacheItem(key, entry), cacheItemPolicy);

            return entry;
        }

        public object Get(string key)
        {
            return ObjectCache[key];
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Update(string key, object entry, DateTime utcExpirey)
        {
            throw new NotImplementedException();
        }
    }

}