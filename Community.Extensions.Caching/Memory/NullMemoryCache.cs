using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Community.Extensions.Caching.Memory
{
    public class NullMemoryCache<TCacheInstance> : IMemoryCache<TCacheInstance> 

    {
        public void Dispose()
        {
        }

        public TObject GetValue<TObject>(string key) where TObject : class
        {
            return null;
        }

        public void SetValue<TObject>(string key, TObject value, MemoryCacheEntryOptions options = null) where TObject : class
        {

        }

        public void Remove(string key)
        {

        }

        public Task<TObject> GetOrSetValueAsync<TObject>(string key, Func<Task<TObject>> factory, MemoryCacheEntryOptions options = null) where TObject : class
        {
            return factory();
        }

        public TObject GetOrSetValue<TObject>(string key, Func<TObject> factory, MemoryCacheEntryOptions options = null) where TObject : class
        {
            return factory();
        }
    }    
}