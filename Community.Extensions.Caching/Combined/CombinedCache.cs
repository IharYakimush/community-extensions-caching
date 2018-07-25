using System;
using Community.Extensions.Caching.Distributed;
using Community.Extensions.Caching.Memory;

namespace Community.Extensions.Caching.Combined
{
    public class CombinedCache<TCacheInstance> : ICombinedCache<TCacheInstance>
    {
        public CombinedCache(IMemoryCache<TCacheInstance> memoryCache, IDistributedCache<TCacheInstance> distributedCache)
        {
            MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            DistributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }
        public IMemoryCache<TCacheInstance> MemoryCache { get; }
        public IDistributedCache<TCacheInstance> DistributedCache { get; }        
    }
}