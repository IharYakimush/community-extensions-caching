using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public interface IMemoryCache<TCacheInstance> 
    {
        void Dispose();
        TObject GetValue<TObject>(string key) where TObject : class;

        void SetValue<TObject>(string key, TObject value, MemoryCacheEntryOptions options = null) where TObject : class;
        void Remove(string key);

        Task<TObject> GetOrSetValueAsync<TObject>(
            string key,
            Func<Task<TObject>> factory,
            MemoryCacheEntryOptions options = null) where TObject : class;

        TObject GetOrSetValue<TObject>(
            string key,
            Func<TObject> factory,
            MemoryCacheEntryOptions options = null) where TObject : class;
    }
}
