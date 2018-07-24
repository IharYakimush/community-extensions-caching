using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching
{
    public interface IMemoryCache<TCacheInstance> 
    {
        void Dispose();
        bool TryGetValue(object key, out object value);
        ICacheEntry CreateEntry(object key);
        void Remove(object key);

        Task<TResult> GetOrAddAsync<TResult>(
            string key,
            Func<Task<TResult>> factory,
            MemoryCacheEntryOptions options);
    }
}
