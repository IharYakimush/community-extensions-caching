using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;

namespace Comminity.Extensions.Caching.Memory
{
    public static class BuilderExtensions
    {
        public static CombinedCacheOptions<TCacheInstance> AddDefaultMemoryCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, MemoryCacheOptions memoryCacheOptions = null)
        {
            options.MemoryCache = new MemoryCache(Options.Create(memoryCacheOptions ?? new MemoryCacheOptions()));

            return options;
        }        
    }
}
