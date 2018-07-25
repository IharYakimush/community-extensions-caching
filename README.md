# Combine In-Memory and Distributed Caching
Standard version of `IMemoryCache` and `IDistributedCache` from `Microsoft.Extensions.Caching` allows to register only 1 instance in `IServiceCollection`. 
This project allows you to:
 - Use generic versions of `IMemoryCache<T>` and `IDistributedCache<T>` to be able to manage multiple cache instances
 - Use strongly typed objects with IDistributedCache<T> instead of byte arrays
 - Use `ICombinedCache<T>` combined cache which try to get value from memory cache first and perform fallback to distributed cache
 - Use App.Metrics to monitor cache hit ration and performance

 # Sample
 ### Register caches
 ```
public void ConfigureServices(IServiceCollection services)
{
    // Register memory cache
    services.AddMemoryCache<MyCache>();

    // Allow to collect App.Metrics for memory cache
    services.AddMetricsToMemoryCache<MyCache>();

    // Register Redis distributed cache
    services.AddRedisDistributedCache<MyCache>(options =>
    {
        options.Configuration = "redis";
        options.InstanceName = "redis";
    });

    // Allow to collect App.Metrics for distributed cache
    services.AddMetricsToDistributedCache<MyCache>();

    // Register combined cache
    services.AddCombinedCache<MyCache>();

    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}
```

### Use caches
```
public class ValuesController : ControllerBase
{
    private readonly IMemoryCache<MyCache> mem;
    private readonly IDistributedCache<MyCache> dist;
    private readonly ICombinedCache<MyCache> comb;

    public ValuesController(
        IMemoryCache<MyCache> onlyMemoryCache,
        IDistributedCache<MyCache> onlyDistributedCache, 
        ICombinedCache<MyCache> combinedCache)
    {
        mem = onlyMemoryCache ?? throw new ArgumentNullException(nameof(onlyMemoryCache));
        dist = onlyDistributedCache ?? throw new ArgumentNullException(nameof(onlyDistributedCache));
        comb = combinedCache ?? throw new ArgumentNullException(nameof(combinedCache));
    }

    public ActionResult<IEnumerable<string>> Get()
    {
        string dateString = DateTime.UtcNow.ToString("O");

        return new string[]
        {
            "v1mem-" + this.mem.GetOrSetValue("v1", () => dateString),

            "v1dist-" + this.dist.GetOrSetValue("v1", () => dateString),

            "v1comb-" + this.comb.GetOrSetValue("v1", () => dateString)
        };
    }
}
```

### Sample project
https://github.com/IharYakimush/community-extensions-caching/tree/master/Community.Extensions.Caching.Sample

# NuGet
 - Base abstractions and memory cache: https://www.nuget.org/packages/Community.Extensions.Caching/
 - Redis distributed cache: https://www.nuget.org/packages/Community.Extensions.Caching.Redis/
 - App.Metrics for caches: https://www.nuget.org/packages/Community.Extensions.Caching.AppMetrics/
