using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Comminity.Extensions.Caching
{
    public class NullMemoryCache<TCacheInstance> : IMemoryCache<TCacheInstance>
    {
        public void Dispose()
        {
        }

        public bool TryGetValue(object key, out object value)
        {
            value = null;
            return false;
        }

        public ICacheEntry CreateEntry(object key)
        {
            return new NullCacheEntry(key);
        }

        public void Remove(object key)
        {            
        }
    }

    public class NullCacheEntry : ICacheEntry
    {
        public NullCacheEntry(object key)
        {
            Key = key;
            ExpirationTokens = new List<IChangeToken>(0);
            PostEvictionCallbacks = new List<PostEvictionCallbackRegistration>(0);
        }
        public void Dispose()
        {

        }

        public object Key { get; }

        public object Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public IList<IChangeToken> ExpirationTokens { get; }
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }
        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }
    }
}