using App.Metrics;
using App.Metrics.Timer;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public class DistributedCacheWithMetrics<TCacheInstance> : DistributedCache<TCacheInstance>
    {
        protected MetricsHelper<TCacheInstance> Helper { get; }

        public DistributedCacheWithMetrics(IDistributedCache inner, IMetrics metrics,
            CacheMetrics allowedMetrics) : base(inner)
        {
            this.Helper = new MetricsHelper<TCacheInstance>(metrics, allowedMetrics);

            if (allowedMetrics.HasFlag(CacheMetrics.HitRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Distributed.HitRatio,
                    Metrics.Distributed.HitCount,
                    Metrics.Distributed.TotalCount);
            }

            if (allowedMetrics.HasFlag(CacheMetrics.ErrorRatio))
            {
                this.Helper.MetricsObj.RegisterOneMinuteRate(
                    Metrics.Distributed.ErrorRatio,
                    Metrics.Distributed.ErrorCount,
                    Metrics.Distributed.TotalCount);
            }
        }

        public override byte[] Get(string key)
        {
            this.Helper.MarkTotalCount(Metrics.Distributed.TotalCount);
            TimerContext timer = this.Helper.GetReadTimer(Metrics.Distributed.ReadTimer);

            using (timer)
            {
                try
                {
                    byte[] result = base.Get(key);

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
                catch
                {
                    timer.TrackUserValue("error");
                    this.Helper.MarkErrorCount(Metrics.Distributed.ErrorCount);
                    throw;
                }
            }
        }

        public override async Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            this.Helper.MarkTotalCount(Metrics.Distributed.TotalCount);
            TimerContext timer = this.Helper.GetReadTimer(Metrics.Distributed.ReadTimer);

            using (timer)
            {
                try
                {
                    byte[] result = await base.GetAsync(key, token);

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
                catch
                {
                    timer.TrackUserValue("error");
                    this.Helper.MarkErrorCount(Metrics.Distributed.ErrorCount);
                    throw;
                }
            }
        }

        public override void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            TimerContext timer = this.Helper.GetWriteTimer(Metrics.Distributed.WriteTimer);

            using (timer)
            {
                try
                {
                    base.Set(key, value, options);
                    timer.TrackUserValue("ok");
                }
                catch
                {
                    timer.TrackUserValue("error");
                    throw;
                }                
            }
        }

        public override async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            TimerContext timer = this.Helper.GetWriteTimer(Metrics.Distributed.WriteTimer);

            using (timer)
            {
                try
                {
                    await base.SetAsync(key, value, options, token);
                    timer.TrackUserValue("ok");
                }
                catch
                {
                    timer.TrackUserValue("error");
                    throw;
                }
            }
        }              
    }
}
