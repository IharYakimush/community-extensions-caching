using System;
using System.Collections.Generic;
using System.Timers;
using App.Metrics;
using App.Metrics.Gauge;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public static class Metrics
    {
        private static readonly Timer Timer = new Timer { Interval = 60000 };

        private static readonly LinkedList<Tuple<IMetrics, GaugeOptions, MeterOptions, MeterOptions>> Ratios =
            new LinkedList<Tuple<IMetrics, GaugeOptions, MeterOptions, MeterOptions>>();

        public static IMetrics RegisterOneMinuteRate(this IMetrics metrics, GaugeOptions ratio, MeterOptions hit, MeterOptions total)
        {
            if (ratio == null) throw new ArgumentNullException(nameof(ratio));
            if (hit == null) throw new ArgumentNullException(nameof(hit));
            if (total == null) throw new ArgumentNullException(nameof(total));

            Ratios.AddLast(new Tuple<IMetrics, GaugeOptions, MeterOptions, MeterOptions>(metrics, ratio, hit, total));

            if (!Timer.Enabled)
            {
                Timer.Start();
            }

            return metrics;
        }        

        static Metrics()
        {
            Timer.Elapsed += Timer_Elapsed;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var ratio in Ratios)
            {
                try
                {
                    ratio.Item1.Measure.Gauge.SetValue(ratio.Item2,
                        () => new HitRatioGauge(ratio.Item1.Provider.Meter.Instance(ratio.Item3),
                            ratio.Item1.Provider.Meter.Instance(ratio.Item4), m => m.OneMinuteRate));
                }
                catch
                {
                }                
            }
        }

        public static class Distributed
        {
            public static TimerOptions ReadTimer { get; } =
                new TimerOptions
                {
                    Name = "r_time",
                    MeasurementUnit = Unit.Requests,
                    RateUnit = TimeUnit.Milliseconds,
                    Context = "Cache.Distributed"
                };

            public static TimerOptions WriteTimer { get; } =
                new TimerOptions
                {
                    Name = "w_time",
                    MeasurementUnit = Unit.Requests,
                    RateUnit = TimeUnit.Milliseconds,
                    Context = "Cache.Distributed"
                };

            public static MeterOptions HitCount { get; } = new MeterOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Distributed",
                Name = "h_count"
            };

            public static GaugeOptions HitRatio { get; } = new GaugeOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Distributed",
                Name = "h_ratio"
            };

            public static MeterOptions ErrorCount { get; } = new MeterOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Distributed",
                Name = "e_count"
            };

            public static GaugeOptions ErrorRatio { get; } = new GaugeOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Distributed",
                Name = "e_ratio"
            };

            public static MeterOptions TotalCount { get; } = new MeterOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Distributed",
                Name = "t_count"
            };

            public static TimerOptions FactoryTimer { get; } =
                new TimerOptions
                {
                    Name = "f_time",
                    MeasurementUnit = Unit.Requests,
                    RateUnit = TimeUnit.Milliseconds,
                    Context = "Cache.Distributed"
                };
        }

        public static class Memory
        {            
            public static MeterOptions HitCount { get; } = new MeterOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Memory",
                Name = "h_count"
            };

            public static MeterOptions TotalCount { get; } = new MeterOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Memory",
                Name = "t_count"
            };

            public static GaugeOptions HitRatio { get; } = new GaugeOptions
            {
                MeasurementUnit = Unit.Calls,
                Context = "Cache.Memory",
                Name = "h_ratio"
            };

            public static TimerOptions FactoryTimer { get; } =
                new TimerOptions
                {
                    Name = "f_time",
                    MeasurementUnit = Unit.Requests,
                    RateUnit = TimeUnit.Milliseconds,
                    Context = "Cache.Memory"
                };
        }        
    }
}