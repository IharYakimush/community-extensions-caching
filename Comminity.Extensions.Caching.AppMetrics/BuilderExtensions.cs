using App.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddMetricsToMemoryCache<TCacheInstance>(this IServiceCollection services, CacheMetrics allowedMetrics = CacheMetrics.HitRatio)
        {
            services.TryAddSingleton(
                new AllowedMetrics<MemoryCacheWithMetrics<TCacheInstance>> {AllowedCacheMetrics = allowedMetrics});

            if (services.All(d => d.ServiceType != typeof(MemoryCacheOptions<TCacheInstance>)))
            {
                throw new InvalidOperationException($"Unable to add metrics to memory cache, because {typeof(MemoryCacheOptions<TCacheInstance>).FullName} not registered. Try to register normal memory cache first.");
            }

            if (services.All(d => d.ServiceType != typeof(IMetrics)))
            {
                throw new InvalidOperationException($"Unable to add metrics to memory cache, because {typeof(IMetrics).FullName} not registered. Try to register Metrics first.");
            }

            services.RemoveAll<IMemoryCache<TCacheInstance>>();
            services.TryAddSingleton<IMemoryCache<TCacheInstance>, MemoryCacheWithMetrics<TCacheInstance>>();

            return services;
        }

        public static IServiceCollection AddMetricsToDistributedCache<TCacheInstance>(this IServiceCollection services, CacheMetrics allowedMetrics = CacheMetrics.HitRatio | CacheMetrics.AllTime | CacheMetrics.ErrorRatio)
        {
            services.TryAddSingleton(
                new AllowedMetrics<DistributedCacheWithMetrics<TCacheInstance>> { AllowedCacheMetrics = allowedMetrics });

            if (services.All(d => d.ServiceType != typeof(DistributedCacheOptions<TCacheInstance>)))
            {
                throw new InvalidOperationException($"Unable to add metrics to distributed cache, because {typeof(DistributedCacheOptions<TCacheInstance>).FullName} not registered. Try to register normal distributed cache first.");
            }

            if (services.All(d => d.ServiceType != typeof(IMetrics)))
            {
                throw new InvalidOperationException($"Unable to add metrics to distributed cache, because {typeof(IMetrics).FullName} not registered. Try to register Metrics first.");
            }

            services.RemoveAll<IDistributedCache<TCacheInstance>>();
            services.TryAddSingleton<IDistributedCache<TCacheInstance>, DistributedCacheWithMetrics<TCacheInstance>>();

            return services;
        }
    }
}
