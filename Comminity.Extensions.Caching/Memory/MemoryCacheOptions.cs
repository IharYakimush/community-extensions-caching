using System;
using Comminity.Extensions.Caching.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Comminity.Extensions.Caching
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