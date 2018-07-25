using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Community.Extensions.Caching.Distributed
{
    public interface IDistributedCache<T> 
    {
        TObject GetValue<TObject>(string key) where TObject : class;
        Task<TObject> GetValueAsync<TObject>(string key, CancellationToken token = default(CancellationToken)) where TObject : class;

        void SetValue<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null)
            where TObject : class;
        Task SetValueAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken))
            where TObject : class;

        TObject GetOrSetValue<TObject>(string key, Func<TObject> valueFactory,
            DistributedCacheEntryOptions options = null) where TObject : class;
        Task<TObject> GetOrSetValueAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken))
            where TObject : class;

        void Refresh(string key);
        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));

        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));
    }
}