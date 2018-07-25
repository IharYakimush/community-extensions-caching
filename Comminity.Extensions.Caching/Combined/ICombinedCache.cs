using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public interface ICombinedCache<TCacheInstance>
    {
        IMemoryCache<TCacheInstance> MemoryCache { get; }

        IDistributedCache<TCacheInstance> DistributedCache { get; }        
    }
}