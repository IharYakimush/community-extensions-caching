using App.Metrics.Timer;

namespace Community.Extensions.Caching.AppMetrics
{
    public static class NullTimerContext 
    {
        public static TimerContext New()
        {
            return new TimerContext(new App.Metrics.Internal.NoOp.NullTimer(), null);
        }        
    }
}