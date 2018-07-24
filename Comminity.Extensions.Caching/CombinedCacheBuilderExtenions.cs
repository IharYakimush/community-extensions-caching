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
                memoryCache = options.MemoryCacheWrapperFactory?.Invoke(options.MemoryCache) ??
                              new MemoryCache<TCacheInstance>(options.MemoryCache);
            }
            services.TryAddSingleton(memoryCache);

            IDistributedObjectCache<TCacheInstance> distributedCache = new NullDistributedObjectCache<TCacheInstance>();
            if (options.DistributedCache != null)
            {
                distributedCache = options.DistributedCacheWrapperFactory?.Invoke(options.DistributedCache) ??
                                   new DistributedObjectCache<TCacheInstance>(options.DistributedCache);
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