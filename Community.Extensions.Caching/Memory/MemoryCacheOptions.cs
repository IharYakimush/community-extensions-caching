using System;
using Community.Extensions.Caching.Common;
using Microsoft.Extensions.Caching.Memory;

namespace Community.Extensions.Caching.Memory
{
    public class MemoryCacheOptions<TCacheInstance> : CommonCacheOptions<TCacheInstance>
    {
        public IMemoryCache Inner { get; }

        public MemoryCacheOptions(IMemoryCache inner)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions { get; } = Defaults.MemoryCacheEntryOptions;
    }
}