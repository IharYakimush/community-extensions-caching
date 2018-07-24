using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comminity.Extensions.Caching
{
    public static class CombinedCacheBuilderExtenions
    {
        public static IServiceCollection AddCombinedCache<TCacheInstance>(this IServiceCollection services, Action<CombinedCacheOptions<TCacheInstance>> settings)
        {
            CombinedCacheOptions<TCacheInstance> options = new CombinedCacheOptions<TCacheInstance>();

            settings?.Invoke(options);
            IMemoryCache <TCacheInstance> memoryCache = new NullMemoryCache<TCacheInstance>();
            if (options.MemoryCache != null)
            {
                memoryCache = new MemoryCache<TCacheInstance>(options.MemoryCache);
                memoryCache = options?.MemoryCacheWrapperFactory(memoryCache) ?? memoryCache;
            }
            services.TryAddSingleton(memoryCache);

            IDistributedCache<TCacheInstance> distributedCache = new NullDistributedCache<TCacheInstance>();
            if (options.DistributedCache != null)
            {
                distributedCache = new DistributedCache<TCacheInstance>(options.DistributedCache);
                distributedCache = options?.DistributedCacheWrapperFactory(distributedCache) ?? distributedCache;
            }                
            services.TryAddSingleton(distributedCache);

            CombinedCache<TCacheInstance> combinedCache = new CombinedCache<TCacheInstance>(memoryCache, distributedCache)
            {
                DefaultMemoryCacheEntryOptions = options.DefaultMemoryCacheEntryOptions ??
                                                           Defaults.MemoryCacheEntryOptions,
                DefaultDistributedCacheEntryOptions = options.DefaultDistributedCacheEntryOptions ??
                                                      Defaults.DistributedCacheEntryOptions,
                FinalKeyFactory = Defaults.FinalKeyFactory,

                Serializer = options.Serializer ?? Defaults.Serializer,

                Deserializer = options.Deserializer ?? Defaults.Deserializer
            };

            services.TryAddSingleton<ICombinedCache<TCacheInstance>>(combinedCache);

            return services;
        }        
    }
}