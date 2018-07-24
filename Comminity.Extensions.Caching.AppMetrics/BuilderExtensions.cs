using App.Metrics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public static class BuilderExtensions
    {
        public static CombinedCacheOptions<TCacheInstance> AddMetricsToMemoryCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            Func<IMemoryCache<TCacheInstance>, IMemoryCache<TCacheInstance>> prev = options.MemoryCacheWrapperFactory;

            options.MemoryCacheWrapperFactory = (mc) => new MemoryCacheWithMetrics<TCacheInstance>((prev?.Invoke(mc) ?? mc), metrics);

            return options;
        }

        public static CombinedCacheOptions<TCacheInstance> AddMetricsToDistributedCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, IMetrics metrics)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            Func<IDistributedCache<TCacheInstance>, IDistributedCache<TCacheInstance>> prev = options.DistributedCacheWrapperFactory;

            options.DistributedCacheWrapperFactory = (mc) => new DistributedCacheWithMetrics<TCacheInstance>((prev?.Invoke(mc) ?? mc), metrics);

            return options;
        }
    }
}
