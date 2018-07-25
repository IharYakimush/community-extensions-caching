using System;
using System.Threading;
using System.Threading.Tasks;
using Comminity.Extensions.Caching.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class DistributedCache<TCacheInstance> : CacheBase<TCacheInstance,DistributedCacheOptions<TCacheInstance>>, IDistributedCache<TCacheInstance>
    {
        private readonly IDistributedCache _inner;

        public DistributedCache(DistributedCacheOptions<TCacheInstance> options) :
            base(options)
        {
            _inner = options.Inner;
        }

        public virtual TObject GetValue<TObject>(string key) where TObject : class
        {
            try
            {
                byte[] data = this._inner.Get(key);

                return HandleGet<TObject>(data, this.Options.Deserializer);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (this.Options.GetError(exception))
                {
                    return null;
                }

                throw;
            }            
        }

        public virtual async Task<TObject> GetValueAsync<TObject>(string key, CancellationToken token = default(CancellationToken)) where TObject : class
        {
            try
            {
                byte[] data = await this._inner.GetAsync(key, token);

                return HandleGet<TObject>(data, this.Options.Deserializer);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (this.Options.GetError(exception))
                {
                    return null;
                }

                throw;
            }
        }

        public virtual void SetValue<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null) where TObject : class
        {            
            try
            {
                byte[] data = HandleSet(value, this.Options.Serializer);

                this._inner.Set(key, data, options ?? this.Options.DefaultCacheEntryOptions);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.SetError(exception))
                {
                    throw;
                }                
            }
        }

        public virtual async Task SetValueAsync<TObject>(string key, TObject value, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken)) where TObject : class
        {
            try
            {
                byte[] data = HandleSet(value, this.Options.Serializer);

                await this._inner.SetAsync(key, data, options ?? this.Options.DefaultCacheEntryOptions, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.SetError(exception))
                {
                    throw;
                }
            }
        }

        public virtual async Task<TObject> GetOrSetValueAsync<TObject>(string key, Func<Task<TObject>> valueFactory, DistributedCacheEntryOptions options = null,
            CancellationToken token = default(CancellationToken)) where TObject : class
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            TObject value = await this.GetValueAsync<TObject>(key, token);

            if (value == null)
            {
                value = await valueFactory();

                await this.SetValueAsync(key, value, options, token);
            }

            return value;
        }

        public virtual TObject GetOrSetValue<TObject>(string key, Func<TObject> valueFactory, DistributedCacheEntryOptions options = null) where TObject : class
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            TObject value = this.GetValue<TObject>(key);

            if (value == null)
            {
                value = valueFactory();

                this.SetValue(key, value, options);
            }

            return value;
        }

        public virtual void Refresh(string key)
        {
            try
            {
                _inner.Refresh(key);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.RefreshError(exception))
                {
                    throw;
                }
            }            
        }

        public virtual async Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
        {
            try
            {
                await _inner.RefreshAsync(key, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.RefreshError(exception))
                {
                    throw;
                }
            }
        }

        public virtual void Remove(string key)
        {
            try
            {
                _inner.Remove(key);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.RemoveError(exception))
                {
                    throw;
                }
            }            
        }

        public virtual async Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
        {
            try
            {
                await _inner.RemoveAsync(key, token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!this.Options.RemoveError(exception))
                {
                    throw;
                }
            }   
        }

        private TObject HandleGet<TObject>(byte[] data, Func<byte[], object> deserialize)
            where TObject : class
        {
            if (data == null)
            {
                return default(TObject);
            }

            if (deserialize != null)
            {
                return (TObject)deserialize(data);
            }

            return (TObject)Defaults.Deserializer(data);
        }

        private byte[] HandleSet<TObject>(TObject value, Func<object, byte[]> serialize)
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
    }
}