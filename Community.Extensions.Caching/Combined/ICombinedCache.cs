using Community.Extensions.Caching.Distributed;
using Community.Extensions.Caching.Memory;

namespace Community.Extensions.Caching.Combined
{
    public interface ICombinedCache<TCacheInstance>
    {
        IMemoryCache<TCacheInstance> MemoryCache { get; }

        IDistributedCache<TCacheInstance> DistributedCache { get; }        
    }
}