using System;
using System.Threading;
using System.Threading.Tasks;
using Comminity.Extensions.Caching.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Comminity.Extensions.Caching
{
    public class MemoryCache<TCacheInstance> : CacheBase<TCacheInstance, MemoryCacheOptions<TCacheInstance>>, IMemoryCache<TCacheInstance>
    {
        private readonly IMemoryCache _inner;

        public MemoryCache(MemoryCacheOptions<TCacheInstance> options):base(options)
        {
            _inner = options.Inner;
        }

        public virtual void Dispose()
        {
            _inner.Dispose();
        }

        public virtual TObject GetValue<TObject>(string key) where TObject : class
        {
            key = this.EnsureCorrectKey(key);
            if (_inner.TryGetValue(key, out object value))
            {
                return value as TObject;
            }

            return null;
        }

        public virtual void SetValue<TObject>(string key, TObject value, MemoryCacheEntryOptions options = null) where TObject : class
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            key = this.EnsureCorrectKey(key);
            options = options ?? this.Options.DefaultMemoryCacheEntryOptions;

            _inner.Set(key, value, options);
        }

        public virtual void Remove(string key)
        {
            key = this.EnsureCorrectKey(key);
            _inner.Remove(key);
        }

        public virtual async Task<TObject> GetOrSetValueAsync<TObject>(
            string key,
            Func<Task<TObject>> factory,
            MemoryCacheEntryOptions options = null) where TObject : class
        {            
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            TObject result = this.GetValue<TObject>(key);

            if (result != null)
            {
                return result;
            }

            result = await factory();

            this.SetValue(key, result, options);
            
            return result;
        }

        public virtual TObject GetOrSetValue<TObject>(string key, Func<TObject> factory, MemoryCacheEntryOptions options = null) where TObject : class
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            TObject result = this.GetValue<TObject>(key);

            if (result != null)
            {
                return result;
            }

            result = factory();

            this.SetValue(key, result, options);

            return result;
        }
    }
}