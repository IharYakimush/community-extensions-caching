using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

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
                await combinedCache.DistributedCache.RemoveDistributedAsync<TResult, TCacheInstance>(key, finalKey,
                    cancellationToken);
            }
            finally
            {
                combinedCache.MemoryCache.RemoveMemory<TResult, TCacheInstance>(key, finalKey);
            }
        }

        public static void RemoveMemory<TResult, TCacheInstance>(
            this IMemoryCache<TCacheInstance> memoryCache,
            string key,
            string finalKey = null)
        {
            finalKey = finalKey ?? Defaults.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);

            memoryCache.Remove(finalKey);
        }

        public static Task RemoveDistributedAsync<TResult, TCacheInstance>(
            this IDistributedCache<TCacheInstance> distributedCache,
            string key,
            string finalKey = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            finalKey = finalKey ?? Defaults.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);

            return distributedCache.RemoveAsync(finalKey, cancellationToken);
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
                distributed = () => GetOrAddDistributed(
                    combinedCache.DistributedCache, 
                    key,
                    distributedCacheEntryOptions ?? combinedCache.DefaultDistributedCacheEntryOptions, 
                    factory,
                    finalKey, 
                    combinedCache.Deserializer, 
                    combinedCache.Serializer, 
                    cancellationToken);
            }

            Func<Task<TResult>> memory = distributed;

            if ((allowedCaches & AllowedCaches.Memory) == AllowedCaches.Memory)
            {
                memory = () => GetOrAddMemory(
                    combinedCache.MemoryCache, 
                    key,
                    memoryCacheEntryOptions ?? combinedCache.DefaultMemoryCacheEntryOptions, 
                    distributed, 
                    finalKey);
            }

            return await memory();
        }

        public static async Task<TResult> GetOrAddMemory<TResult, TCacheInstance>(
            this IMemoryCache<TCacheInstance> memoryCache,
            string key,
            MemoryCacheEntryOptions options,
            Func<Task<TResult>> factory,
            string finalKey = null)
        {
            finalKey = finalKey ?? Defaults.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);

            if (memoryCache.TryGetValue(finalKey,out object value) && value is TResult result)
            {
                return result;
            }

            result = await factory();

            ICacheEntry entry = memoryCache.CreateEntry(finalKey);

            entry.Value = value;
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
            entry.AbsoluteExpiration = options.AbsoluteExpiration ?? Defaults.MemoryCacheEntryOptions.AbsoluteExpiration;
            if (options.ExpirationTokens!= null)
            {
                foreach (IChangeToken token in options.ExpirationTokens)
                {
                    entry.ExpirationTokens.Add(token);
                }
            }

            if (options.PostEvictionCallbacks != null)
            {
                foreach (var item in options.PostEvictionCallbacks)
                {
                    entry.PostEvictionCallbacks.Add(item);
                }
            }

            entry.Priority = options.Priority;
            entry.Size = options.Size;
            entry.SlidingExpiration = options.SlidingExpiration;

            return result;
        }

        public static async Task<TResult> GetOrAddDistributed<TResult, TCacheInstance>(
            this IDistributedCache<TCacheInstance> distributedCache,
            string key,
            DistributedCacheEntryOptions options,
            Func<Task<TResult>> factory,
            string finalKey = null,
            Func<byte[], object> deserializer = null,
            Func<object,byte[]> serializer = null,
            CancellationToken cancellationToken = default(CancellationToken))
        where TResult : class 
        {
            finalKey = finalKey ?? Defaults.FinalKeyFactory(typeof(TCacheInstance), typeof(TResult), key);
            serializer = serializer ?? Defaults.Serializer;
            deserializer = deserializer ?? Defaults.Deserializer;

            byte[] data = await distributedCache.GetAsync(finalKey, cancellationToken);
            TResult result;
            if (data != null)
            {
                try
                {
                    result = deserializer(data) as TResult;
                    return result;
                }
                catch
                {
                    // return new cache entry if previous one if corrupted
                }
            }

            result = await factory();

            try
            {
                data = serializer(result);
                await distributedCache.SetAsync(finalKey, data, options, cancellationToken);
            }
            catch
            {
                // Ignore cache exceptions
            }

            return result;
        }
    }
}