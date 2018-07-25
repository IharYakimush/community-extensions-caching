using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public static class CombinedCacheExtensions
    {
        public static void Remove<TCacheInstance>(this ICombinedCache<TCacheInstance> cache, string key)
        {
            try
            {
                cache.DistributedCache.Remove(key);
            }
            finally
            {
                cache.MemoryCache.Remove(key);
            }
        }

        public static async Task RemoveAsync<TCacheInstance>(this ICombinedCache<TCacheInstance> cache, string key, CancellationToken token = default(CancellationToken))
        {
            try
            {
                await cache.DistributedCache.RemoveAsync(key, token);
            }
            finally
            {
                cache.MemoryCache.Remove(key);
            }
        }

        public static TObject GetOrSetValue<TCacheInstance,TObject>(
            this ICombinedCache<TCacheInstance> cache, 
            string key, 
            Func<TObject> valueFactory,
            MemoryCacheEntryOptions memoryOptions = null,
            DistributedCacheEntryOptions disrtibutedOptions = null) where TObject : class
        {
            return cache.MemoryCache.GetOrSetValue(key,
                () => cache.DistributedCache.GetOrSetValue(key, valueFactory, disrtibutedOptions), memoryOptions);
        }

        public static Task<TObject> GetOrSetValueAsync<TCacheInstance,TObject>(
            this ICombinedCache<TCacheInstance> cache, 
            string key, 
            Func<Task<TObject>> valueFactory,
            MemoryCacheEntryOptions memoryOptions = null,
            DistributedCacheEntryOptions distributedOptions = null,
            CancellationToken token = default(CancellationToken))
            where TObject : class
        {
            return cache.MemoryCache.GetOrSetValueAsync(key,
                () => cache.DistributedCache.GetOrSetValueAsync(key, valueFactory, distributedOptions, token), memoryOptions);
        }
    }    
}