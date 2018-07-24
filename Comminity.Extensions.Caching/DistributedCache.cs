using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class DistributedCache<TCacheInstance> : IDistributedCache<TCacheInstance>
    {
        public virtual byte[] Get(string key)
        {
            return _inner.Get(key);
        }

        public virtual Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
        {
            return _inner.GetAsync(key, token);
        }

        public virtual void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _inner.Set(key, value, options);
        }

        public virtual Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
            CancellationToken token = new CancellationToken())
        {
            return _inner.SetAsync(key, value, options, token);
        }

        public virtual void Refresh(string key)
        {
            _inner.Refresh(key);
        }

        public virtual Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            return _inner.RefreshAsync(key, token);
        }

        public virtual void Remove(string key)
        {
            _inner.Remove(key);
        }

        public virtual Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
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