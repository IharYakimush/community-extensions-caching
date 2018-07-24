using System;
using App.Metrics;
using App.Metrics.Timer;

namespace Comminity.Extensions.Caching.AppMetrics
{
    public static class NullTimerContext 
    {
        public static TimerContext New()
        {
            return new TimerContext(new App.Metrics.Internal.NoOp.NullTimer(), null);
        }        
    }
}