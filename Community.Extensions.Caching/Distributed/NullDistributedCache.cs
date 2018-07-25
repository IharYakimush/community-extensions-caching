using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Community.Extensions.Caching.Distributed
{
    public class NullDistributedCache<TCacheInstance> : IDistributedCache<TCacheInstance>
    {
        public TObject GetValue<TObject>(string key) where TObject : class
        {
            return null;
        }

        public Task<TObject> GetValueAsync<TObject>(string key, CancellationToken token = default(CancellationToken)) where TObject : class
        {
            return Task.FromResult<TObject>(null);
        }

        public void SetValue<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null) where TObject : class
        {

        }

        public Task SetValueAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken)) where TObject : class
        {
            return Task.CompletedTask;
        }

        public TObject GetOrSetValue<TObject>(string key, Func<TObject> valueFactory, DistributedCacheEntryOptions options = null) where TObject : class
        {
            return valueFactory();
        }

        public Task<TObject> GetOrSetValueAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken)) where TObject : class
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