using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class DistributedObjectCache<TCacheInstance> : IDistributedObjectCache<TCacheInstance>
    {
        public async Task<TObject> GetOrSetAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options,
            CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null, Func<byte[], TObject> deserialize = null) 
            where TObject : class
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));
            if (options == null) throw new ArgumentNullException(nameof(options));

            TObject value = await this.GetAsync(key, token, deserialize);

            if (value == null)
            {
                value = await valueFactory();

                await this.SetAsync(key, value, options, token, serialize);
            }

            return value;
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

        public virtual TObject Get<TObject>(string key, Func<byte[], TObject> deserialize = null) where TObject : class
        {
            byte[] data = this._inner.Get(key);

            return HandleGet(data, deserialize);
        }

        private TObject HandleGet<TObject>(byte[] data, Func<byte[], TObject> deserialize)
        {
            if (data == null)
            {
                return default(TObject);
            }

            if (deserialize != null)
            {
                return deserialize(data);
            }

            return (TObject)Defaults.Deserializer(data);
        }

        private byte[] HandleSet<TObject>(TObject value, Func<TObject, byte[]> serialize)
            where TObject : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (serialize != null)
            {
                return serialize(value) ?? throw new ArgumentException($"Unable to serialize object of type {typeof(TObject)} using provided serializer. Result of serialization was null.", nameof(serialize));
            }

            return Defaults.Serializer(value);
        }

        public virtual async Task<TObject> GetAsync<TObject>(string key, CancellationToken token = default(CancellationToken), Func<byte[], TObject> deserialize = null) where TObject : class
        {
            byte[] data = await this._inner.GetAsync(key, token);

            return HandleGet(data, deserialize);
        }

        public virtual void Set<TObject>(string key, TObject value, DistributedCacheEntryOptions options, Func<TObject, byte[]> serialize = null)
            where TObject : class
        {
            byte[] data = HandleSet(value, serialize);

            this._inner.Set(key, data, options);
        }

        public virtual async Task SetAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken), Func<TObject, byte[]> serialize = null)
            where TObject : class
        {
            byte[] data = HandleSet(value, serialize);

            await this._inner.SetAsync(key, data, options, token);
        }

        private readonly IDistributedCache _inner;

        public DistributedObjectCache(IDistributedCache inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
    }
}