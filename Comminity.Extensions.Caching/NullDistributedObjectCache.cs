using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class NullDistributedObjectCache<TCacheInstance> : IDistributedObjectCache<TCacheInstance>
    {
        public TObject Get<TObject>(string key, Func<byte[], TObject> deserialize = null) where TObject : class
        {
            return null;
        }

        public Task<TObject> GetAsync<TObject>(string key, CancellationToken token = default(CancellationToken), Func<byte[], TObject> deserialize = null) where TObject : class
        {
            return Task.FromResult<TObject>(null);
        }

        public void Set<TObject>(string key, TObject value, DistributedCacheEntryOptions options, Func<TObject, byte[]> serialize = null) where TObject : class
        {

        }

        public Task SetAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null) where TObject : class
        {
            return Task.CompletedTask;
        }

        public Task<TObject> GetOrSetAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null, Func<byte[], TObject> deserialize = null) where TObject : class
        {
            return valueFactory();
        }

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
        }

        public Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}