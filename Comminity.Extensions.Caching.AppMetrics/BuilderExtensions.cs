using App.Metrics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public static class BuilderExtensions
    {
        public static CombinedCacheOptions<TCacheInstance> AddMetricsToMemoryCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, IMetrics metrics, CacheMetrics allowedMetrics = CacheMetrics.HitRatio)
        {
            if (metrics == null)
            {
                throw new ArgumentNullException(nameof(metrics));
            }

            options.MemoryCacheWrapperFactory = mc =>
                new MemoryCacheWithMetrics<TCacheInstance>(mc, metrics, allowedMetrics);

            return options;
        }

        //public static CombinedCacheOptions<TCacheInstance> AddMetricsToDistributedCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, IMetrics metrics, CacheMetrics allowedMetrics = CacheMetrics.HitRatio | CacheMetrics.ErrorRatio | CacheMetrics.AllTime)
        //{
        //    if (metrics == null)
        //    {
        //        throw new ArgumentNullException(nameof(metrics));
        //    }

        //    options.DistributedCacheWrapperFactory = (mc) =>
        //        new DistributedCacheWithMetrics<TCacheInstance>(mc, metrics, allowedMetrics);

        //    return options;
        //}
    }
}
