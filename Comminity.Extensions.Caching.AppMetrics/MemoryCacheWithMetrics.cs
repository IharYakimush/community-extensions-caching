using System;
using System.Linq;
using System.Timers;
using App.Metrics;
using App.Metrics.Gauge;
using App.Metrics.Meter;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public class MemoryCacheWithMetrics<TCacheInstance> : IMemoryCache<TCacheInstance>
    {
        private static MeterOptions Hit = new MeterOptions {
            MeasurementUnit = Unit.Calls,
            Context = "Cache.Memory",
            Name = "h_count"
        };

        private static MeterOptions Total = new MeterOptions
        {
            MeasurementUnit = Unit.Calls,
            Context = "Cache.Memory",
            Name = "t_count"
        };

        private static GaugeOptions HitRatio = new GaugeOptions
        {
            MeasurementUnit = Unit.Calls,
            Context = "Cache.Memory",
            Name = "h_ratio"
        };

        private static MetricTags MetricsTags { get; } = new MetricTags("inst",
            typeof(TCacheInstance).Name.Split('.').Last());

        private readonly IMemoryCache<TCacheInstance> _inner;
        private readonly IMetrics _metrics;
        private readonly Timer timer = new Timer() { Interval = 30000 };

        public MemoryCacheWithMetrics(IMemoryCache<TCacheInstance> inner, IMetrics metrics)
        {
            this._inner = inner ?? throw new ArgumentNullException(nameof(inner));
            this._metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));            
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _metrics.Measure.Gauge.SetValue(HitRatio, () => new HitRatioGauge(_metrics.Provider.Meter.Instance(Hit), _metrics.Provider.Meter.Instance(Total), m => m.OneMinuteRate));
        }

        public void Dispose()
        {
            _inner.Dispose();
            this.timer.Dispose();
        }

        public bool TryGetValue(object key, out object value)
        {
            bool result = _inner.TryGetValue(key, out value);
            this._metrics.Measure.Meter.Mark(Total, MetricsTags);
            if (result)
            {
                this._metrics.Measure.Meter.Mark(Hit, MetricsTags);
            }
            
            return result;
        }

        public ICacheEntry CreateEntry(object key)
        {
            return _inner.CreateEntry(key);
        }

        public void Remove(object key)
        {
            _inner.Remove(key);
        }
    }
}
