using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public interface ICombinedCache<TCacheInstance>
    {
        IMemoryCache<TCacheInstance> MemoryCache { get; }

        IDistributedObjectCache<TCacheInstance> DistributedCache { get; }

        DistributedCacheEntryOptions DefaultDistributedCacheEntryOptions { get; }

        MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; }

        Func<Type,Type,string, string> FinalKeyFactory { get; }

        Func<object, byte[]> Serializer { get; }

        Func<byte[], object> Deserializer { get; }
    }
}