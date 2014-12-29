using System;

namespace CUWebinars.Business.Core.Cache
{
    public interface ICachingService
    {
        object Add(string key, object entry, DateTime expirey);
        object Get(string key);
        void Remove(string key);
        void Update(string key, object entry, DateTime utcExpirey);

    }
}
