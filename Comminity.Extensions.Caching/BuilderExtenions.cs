using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comminity.Extensions.Caching
{
    public static class BuilderExtenions
    {
        public static IServiceCollection AddNullMemoryCache<TCacheInstance>(this IServiceCollection services)
        {
            services.TryAddSingleton<IMemoryCache<TCacheInstance>, NullMemoryCache<TCacheInstance>>();

            return services;
        }

        public static IServiceCollection AddMemoryCache<TCacheInstance>(
            this IServiceCollection services, 
            Action<MemoryCacheOptions> setupInner = null, 
            Action<MemoryCacheOptions<TCacheInstance>> setup = null)
        {
            MemoryCacheOptions innerOptions = new MemoryCacheOptions();
            setupInner?.Invoke(innerOptions);

            MemoryCacheOptions<TCacheInstance> options = new MemoryCacheOptions<TCacheInstance>(new MemoryCache(innerOptions));
            setup?.Invoke(options);

            services.TryAddSingleton(options);
            services.TryAddSingleton<IMemoryCache<TCacheInstance>, MemoryCache<TCacheInstance>>();

            return services;
        }

        public static IServiceCollection AddNullDistributedCache<TCacheInstance>(this IServiceCollection services)
        {
            services.TryAddSingleton<IDistributedCache<TCacheInstance>, NullDistributedCache<TCacheInstance>>();

            return services;
        }

        public static IServiceCollection AddCombinedCache<TCacheInstance>(this IServiceCollection services)
        {            
            services.TryAddSingleton<ICombinedCache<TCacheInstance>,CombinedCache<TCacheInstance>>();

            return services;
        }        
    }    
}