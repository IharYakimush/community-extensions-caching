using System;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public class MemoryCache<TCacheInstance> : IMemoryCache<TCacheInstance>
    {
        public void Dispose()
        {
            _inner.Dispose();
        }

        public bool TryGetValue(object key, out object value)
        {
            return _inner.TryGetValue(key, out value);
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _inner.CreateEntry(key);
        }

        public void Remove(object key)
        {
            _inner.Remove(key);
        }

        private readonly IMemoryCache _inner;

        public MemoryCache(IMemoryCache inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }
}