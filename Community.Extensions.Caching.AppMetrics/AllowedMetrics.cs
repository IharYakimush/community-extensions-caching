namespace Community.Extensions.Caching.AppMetrics
{
    public class AllowedMetrics<T>
    {
        public  CacheMetrics AllowedCacheMetrics { get; set; } = CacheMetrics.None;
    }
}