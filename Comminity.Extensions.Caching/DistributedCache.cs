using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class DistributedCache<TCacheInstance> : IDistributedCache<TCacheInstance>
    {
        public byte[] Get(string key)
        {
            return _inner.Get(key);
        }

        public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            return _inner.GetAsync(key, token);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _inner.Set(key, value, options);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            return _inner.SetAsync(key, value, options, token);
        }

        public void Refresh(string key)
        {
            _inner.Refresh(key);
        }

        public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            return _inner.RefreshAsync(key, token);
        }

        public void Remove(string key)
        {
            _inner.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            return _inner.RemoveAsync(key, token);
        }

        private readonly IDistributedCache _inner;

        public DistributedCache(IDistributedCache inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }
}