using System;
using System.Linq;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public class MetricsHelper<TCacheInstance>
    {
        public IMetrics MetricsObj { get; }
        public CacheMetrics AllowedMetrics { get; }

        public MetricTags MetricsTags { get; protected set; }

        public MetricsHelper(IMetrics metrics, CacheMetrics allowedMetrics)
        {
            MetricsObj = metrics ?? throw new ArgumentNullException(nameof(metrics));
            AllowedMetrics = allowedMetrics;
            this.MetricsTags = new MetricTags("inst",
                typeof(TCacheInstance).Name.Split('.').Last());
        }

        public void MarkTotalCount(MeterOptions meterOptions)
        {
            if (this.AllowedMetrics.HasFlag(CacheMetrics.TotalCount))
            {
                this.MetricsObj.Measure.Meter.Mark(meterOptions, MetricsTags);
            }
        }

        public void MarkHitCount(MeterOptions meterOptions)
        {
            if (this.AllowedMetrics.HasFlag(CacheMetrics.HitCount))
            {
                this.MetricsObj.Measure.Meter.Mark(meterOptions, MetricsTags);
            }
        }

        public void MarkErrorCount(MeterOptions meterOptions)
        {
            if (this.AllowedMetrics.HasFlag(CacheMetrics.ErrorCount))
            {
                this.MetricsObj.Measure.Meter.Mark(meterOptions, MetricsTags);
            }
        }

        public TimerContext GetReadTimer(TimerOptions timerOptions)
        {
            return this.AllowedMetrics.HasFlag(CacheMetrics.ReadTime)
                ? this.MetricsObj.Measure.Timer.Time(timerOptions, MetricsTags)
                : NullTimerContext.New();
        }

        public TimerContext GetFactoryTimer(TimerOptions timerOptions)
        {
            return this.AllowedMetrics.HasFlag(CacheMetrics.FactoryTime)
                ? this.MetricsObj.Measure.Timer.Time(timerOptions, MetricsTags)
                : NullTimerContext.New();
        }

        public TimerContext GetWriteTimer(TimerOptions timerOptions)
        {
            return this.AllowedMetrics.HasFlag(CacheMetrics.WriteTime)
                ? this.MetricsObj.Measure.Timer.Time(timerOptions, MetricsTags)
                : NullTimerContext.New();
        }
    }
}