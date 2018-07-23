using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public interface IDistributedCache<T> 
    {
        byte[] Get(string key);
        Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken));
        void Set(string key, byte[] value, DistributedCacheEntryOptions options);

        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken));

        void Refresh(string key);
        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));
        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));
    }
}