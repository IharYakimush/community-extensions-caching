using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public static class CombinedCacheExtensions
    {
        public static async Task RemoveAsync<TResult, TCacheInstance>(
            this ICombinedCache<TCacheInstance> combinedCache,
            string key,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string finalKey = combinedCache.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);

            try
            {
                await combinedCache.DistributedCache.RemoveAsync(finalKey, cancellationToken);
            }
            finally
            {
                combinedCache.MemoryCache.Remove(finalKey);
            }
        }

        public static async Task<TResult> GetOrAddAsync<TResult, TCacheInstance>(
            this ICombinedCache<TCacheInstance> combinedCache,
            AllowedCaches allowedCaches,
            string key,
            Func<Task<TResult>> factory,
            MemoryCacheEntryOptions memoryCacheEntryOptions = null,
            DistributedCacheEntryOptions distributedCacheEntryOptions = null,
            CancellationToken cancellationToken = default(CancellationToken))
        where TResult : class 
        {
            string finalKey = combinedCache.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);

            Func<Task<TResult>> distributed = factory;

            if ((allowedCaches & AllowedCaches.Distributed) == AllowedCaches.Distributed)
            {
                Func<byte[], TResult> deserialize = null;
                if (combinedCache.Deserializer != null)
                {
                    deserialize = bytes => (TResult)combinedCache.Deserializer(bytes);
                }                

                distributed = () => combinedCache.DistributedCache.GetOrSetAsync(
                    finalKey, 
                    factory,
                    distributedCacheEntryOptions ?? combinedCache.DefaultDistributedCacheEntryOptions, 
                    cancellationToken, 
                    combinedCache.Serializer,
                    deserialize);
            }

            Func<Task<TResult>> memory = distributed;

            if ((allowedCaches & AllowedCaches.Memory) == AllowedCaches.Memory)
            {
                memory = () => combinedCache.MemoryCache.GetOrAddAsync(
                    finalKey,
                    distributed,
                    memoryCacheEntryOptions ?? combinedCache.DefaultMemoryCacheEntryOptions);
            }

            return await memory();
        }        
    }    
}