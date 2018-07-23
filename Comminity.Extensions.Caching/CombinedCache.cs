using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
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
        public DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions { get; set; }

        public MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; set; }
        public Func<Type, Type, string, string> FinalKeyFactory { get; set; }
        public Func<object, byte[]> Serializer { get; set; }
        public Func<byte[], object> Deserializer { get; set; }
    }
}