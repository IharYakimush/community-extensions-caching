using System;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public class MemoryCacheWithMetrics<TCacheInstance> : MemoryCache<TCacheInstance>
    {
        protected MetricsHelper<TCacheInstance> Helper { get; }
        public MemoryCacheWithMetrics(IMemoryCache inner, IMetrics metrics, CacheMetrics allowedMetrics)
            : base(inner)
        {
            this.Helper = new MetricsHelper<TCacheInstance>(metrics, allowedMetrics);
            if (allowedMetrics.HasFlag(CacheMetrics.HitRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Memory.HitRatio,
                    Metrics.Memory.HitCount,
                    Metrics.Memory.TotalCount);
            }
        }

        public override bool TryGetValue(object key, out object value)
        {
            bool result = base.TryGetValue(key, out value);

            this.Helper.MarkTotalCount(Metrics.Memory.TotalCount);
            
            if (result)
            {
                this.Helper.MarkHitCount(Metrics.Memory.HitCount);
            }
            
            return result;
        }

        public override Task<TResult> GetOrAddAsync<TResult>(
            string key,
            Func<Task<TResult>> factory,
            MemoryCacheEntryOptions options)
        {
            Func<Task<TResult>> factoryWithMeasurement = factory;

            if (this.Helper.AllowedMetrics.HasFlag(CacheMetrics.FactoryTime))
            {
                factoryWithMeasurement = async () =>
                {
                    using (var timer = this.Helper.GetFactoryTimer(Metrics.Memory.FactoryTimer))
                    {
                        return await factory();
                    }
                };
            }

            return base.GetOrAddAsync(key, factoryWithMeasurement, options);
        }
    }
}
