using System;
using System.Threading.Tasks;
using App.Metrics;
using Community.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Memory;

namespace Community.Extensions.Caching.AppMetrics
{
    public class MemoryCacheWithMetrics<TCacheInstance> : MemoryCache<TCacheInstance>
    {
        protected MetricsHelper<TCacheInstance> Helper { get; }

        public MemoryCacheWithMetrics(MemoryCacheOptions<TCacheInstance> options, IMetrics metrics,
           AllowedMetrics<MemoryCacheWithMetrics<TCacheInstance>> settings) : base(options)
        {
            this.Helper = new MetricsHelper<TCacheInstance>(metrics, settings.AllowedCacheMetrics);
            if (settings.AllowedCacheMetrics.HasFlag(CacheMetrics.HitRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Memory.HitRatio,
                    Metrics.Memory.HitCount,
                    Metrics.Memory.TotalCount,
                    this.Helper.MetricsTags);
            }
        }

        public override TObject GetValue<TObject>(string key)
        {
            this.Helper.MarkTotalCount(Metrics.Memory.TotalCount);

            TObject result = base.GetValue<TObject>(key);            
            
            if (result != null)
            {
                this.Helper.MarkHitCount(Metrics.Memory.HitCount);
            }
            
            return result;
        }

        public override async Task<TObject> GetOrSetValueAsync<TObject>(
            string key,
            Func<Task<TObject>> factory,
            MemoryCacheEntryOptions options = null)
        {
            Func<Task<TObject>> factoryWithMeasurement = factory;

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

            return await base.GetOrSetValueAsync(key, factoryWithMeasurement, options);
        }

        public override TObject GetOrSetValue<TObject>(
            string key,
            Func<TObject> factory,
            MemoryCacheEntryOptions options = null)
        {
            Func<TObject> factoryWithMeasurement = factory;

            if (this.Helper.AllowedMetrics.HasFlag(CacheMetrics.FactoryTime))
            {
                factoryWithMeasurement = () =>
                {
                    using (var timer = this.Helper.GetFactoryTimer(Metrics.Memory.FactoryTimer))
                    {
                        return factory();
                    }
                };
            }

            return base.GetOrSetValue(key, factoryWithMeasurement, options);
        }
    }
}
