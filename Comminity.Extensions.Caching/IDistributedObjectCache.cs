using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comminity.Extensions.Caching
{
    public interface IDistributedObjectCache<TCacheInstance>
    {
        TObject Get<TObject>(string key,Func<byte[], TObject> deserialize = null) where TObject : class;
        Task<TObject> GetAsync<TObject>(string key, CancellationToken token = default(CancellationToken), Func<byte[], TObject> deserialize = null) where TObject : class;

        void Set<TObject>(string key, TObject value, DistributedCacheEntryOptions options, Func<TObject, byte[]> serialize = null)
            where TObject : class;
        Task SetAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null)
            where TObject : class;

        Task<TObject> GetOrSetAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null, Func<byte[], TObject> deserialize = null)
            where TObject : class;

        void Refresh(string key);
        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));

        void Remove(string key);
        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));
    }
}
