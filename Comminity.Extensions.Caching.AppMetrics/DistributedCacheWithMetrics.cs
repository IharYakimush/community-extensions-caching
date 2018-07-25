using App.Metrics;
using App.Metrics.Timer;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;
using Comminity.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public class DistributedCacheWithMetrics<TCacheInstance> : DistributedCache<TCacheInstance>
    {
        protected MetricsHelper<TCacheInstance> Helper { get; }

        public DistributedCacheWithMetrics(DistributedCacheOptions<TCacheInstance> options, IMetrics metrics,
            AllowedMetrics<DistributedCacheWithMetrics<TCacheInstance>> settings) : base(options)
        {
            this.Helper = new MetricsHelper<TCacheInstance>(metrics, settings.AllowedCacheMetrics);

            if (settings.AllowedCacheMetrics.HasFlag(CacheMetrics.HitRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Distributed.HitRatio,
                    Metrics.Distributed.HitCount,
                    Metrics.Distributed.TotalCount);
            }

            if (settings.AllowedCacheMetrics.HasFlag(CacheMetrics.ErrorRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Distributed.ErrorRatio,
                    Metrics.Distributed.ErrorCount,
                    Metrics.Distributed.TotalCount);
            }

            options.OnGetError += Options_OnGetError;
        }

        private void Options_OnGetError(object sender, ErrorEventArgs e)
        {
            this.Helper.MarkErrorCount(Metrics.Distributed.ErrorCount);
        }

        public override TObject GetValue<TObject>(string key)
        {
            this.Helper.MarkTotalCount(Metrics.Distributed.TotalCount);
            TimerContext timer = this.Helper.GetReadTimer(Metrics.Distributed.ReadTimer);

            using (timer)
            {
                TObject result = base.GetValue<TObject>(key);

                if (result != null)
                {
                    timer.TrackUserValue("hit");
                    this.Helper.MarkHitCount(Metrics.Distributed.HitCount);
                }
                else
                {
                    timer.TrackUserValue("miss");
                }

                return result;
            }
        }

        public override async Task<TObject> GetValueAsync<TObject>(string key, CancellationToken token = default(CancellationToken))
        {
            this.Helper.MarkTotalCount(Metrics.Distributed.TotalCount);
            TimerContext timer = this.Helper.GetReadTimer(Metrics.Distributed.ReadTimer);

            using (timer)
            {
                TObject result = await base.GetValueAsync<TObject>(key, token);

                if (result != null)
                {
                    timer.TrackUserValue("hit");
                    this.Helper.MarkHitCount(Metrics.Distributed.HitCount);
                }
                else
                {
                    timer.TrackUserValue("miss");
                }

                return result;
            }
        }

        public override void SetValue<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null)
        {
            TimerContext timer = this.Helper.GetWriteTimer(Metrics.Distributed.WriteTimer);

            using (timer)
            {
                base.SetValue(key, value, options);
                timer.TrackUserValue("ok");           
            }
        }

        public override async Task SetValueAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken))
        {
            TimerContext timer = this.Helper.GetWriteTimer(Metrics.Distributed.WriteTimer);

            using (timer)
            {
                await base.SetValueAsync(key, value, options, token);
                timer.TrackUserValue("ok");
            }
        }

        public override TObject GetOrSetValue<TObject>(string key, Func<TObject> valueFactory,
            DistributedCacheEntryOptions options = null)
        {
            Func<TObject> factoryWithMeasurement = valueFactory;

            if (this.Helper.AllowedMetrics.HasFlag(CacheMetrics.FactoryTime))
            {
                factoryWithMeasurement = () =>
                {
                    using (var timer = this.Helper.GetFactoryTimer(Metrics.Distributed.FactoryTimer))
                    {
                        return valueFactory();
                    }
                };
            }

            return base.GetOrSetValue(key, factoryWithMeasurement, options);
        }

        public override async Task<TObject> GetOrSetValueAsync<TObject>(string key, Func<Task<TObject>> valueFactory,
            DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken))
        {
            Func<Task<TObject>> factoryWithMeasurement = valueFactory;

            if (this.Helper.AllowedMetrics.HasFlag(CacheMetrics.FactoryTime))
            {
                factoryWithMeasurement = async () =>
                {
                    using (var timer = this.Helper.GetFactoryTimer(Metrics.Distributed.FactoryTimer))
                    {
                        return await valueFactory();
                    }
                };
            }

            return await base.GetOrSetValueAsync(key, factoryWithMeasurement, options, token);
        }
    }
}
