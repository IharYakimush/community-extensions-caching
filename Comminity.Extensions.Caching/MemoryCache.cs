using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Comminity.Extensions.Caching
{
    public class MemoryCache<TCacheInstance> : IMemoryCache<TCacheInstance>
    {
        private readonly IMemoryCache _inner;

        public MemoryCache(IMemoryCache inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public virtual void Dispose()
        {
            _inner.Dispose();
        }

        public virtual bool TryGetValue(object key, out object value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _inner.TryGetValue(key, out value);
        }

        public virtual ICacheEntry CreateEntry(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _inner.CreateEntry(key);
        }

        public virtual void Remove(object key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _inner.Remove(key);
        }

        public virtual async Task<TResult> GetOrAddAsync<TResult>(
            string key,
            Func<Task<TResult>> factory,
            MemoryCacheEntryOptions options)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (this.TryGetValue(key, out object value) && value is TResult result)
            {
                return result;
            }

            result = await factory();

            ICacheEntry entry = this.CreateEntry(key);

            entry.Value = value;
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
            entry.AbsoluteExpiration = options.AbsoluteExpiration;
            if (options.ExpirationTokens != null)
            {
                foreach (IChangeToken token in options.ExpirationTokens)
                {
                    entry.ExpirationTokens.Add(token);
                }
            }

            if (options.PostEvictionCallbacks != null)
            {
                foreach (var item in options.PostEvictionCallbacks)
                {
                    entry.PostEvictionCallbacks.Add(item);
                }
            }

            entry.Priority = options.Priority;
            entry.Size = options.Size;
            entry.SlidingExpiration = options.SlidingExpiration;

            return result;
        }        
    }
}