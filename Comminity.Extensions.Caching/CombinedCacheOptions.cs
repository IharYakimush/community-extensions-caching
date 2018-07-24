using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public class CombinedCacheOptions<TCacheInstance>
    {
        public IMemoryCache MemoryCache { get; set; }

        public IDistributedCache DistributedCache { get; set; }

        public DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions { get; set; }

        public MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; set; }

        public Func<Type, Type, string, string> FinalKeyFactory { get; } = null;

        public Func<object, byte[]> Serializer { get; set; } = null;

        public Func<byte[], object> Deserializer { get; set; } = null;

        public Func<IMemoryCache, IMemoryCache<TCacheInstance>> MemoryCacheWrapperFactory { get; set; } = null;

        public Func<IDistributedCache, IDistributedObjectCache<TCacheInstance>> DistributedCacheWrapperFactory { get; set; } = null;
    }
}