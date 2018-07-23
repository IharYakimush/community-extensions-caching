using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comminity.Extensions.Caching
{
    public static class CombinedCacheBuilderExtenions
    {
        public static IServiceCollection AddCombinedCache<TCacheInstance>(this IServiceCollection services, Action<CombinedCacheOptions> settings)
        {
            CombinedCacheOptions options = new CombinedCacheOptions();

            settings?.Invoke(options);

            IMemoryCache<TCacheInstance> memoryCache = options.MemoryCache != null
                ? (IMemoryCache<TCacheInstance>) new MemoryCache<TCacheInstance>(options.MemoryCache)
                : new NullMemoryCache<TCacheInstance>();
            services.TryAddSingleton<IMemoryCache<TCacheInstance>>(memoryCache);

            IDistributedCache<TCacheInstance> distributedCache = options.DistributedCache != null
                ? (IDistributedCache<TCacheInstance>)new DistributedCache<TCacheInstance>(options.DistributedCache)
                : new NullDistributedCache<TCacheInstance>();

            services.TryAddSingleton<IMemoryCache<TCacheInstance>>(memoryCache);
            services.TryAddSingleton<IDistributedCache<TCacheInstance>>(distributedCache);

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