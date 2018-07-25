using System;
using System.Dynamic;
using Comminity.Extensions.Caching.Common;
using Comminity.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Distributed;

namespace Comminity.Extensions.Caching
{
    public class DistributedCacheOptions<TCacheInstance> : CommonCacheOptions<TCacheInstance>
    {
        public DistributedCacheOptions(IDistributedCache inner)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }
        public IDistributedCache Inner { get; }
        public DistributedCacheEntryOptions DefaultCacheEntryOptions { get; } = Defaults.DistributedCacheEntryOptions;
        public Func<object, byte[]> Serializer { get; set; } = Defaults.Serializer;
        public Func<byte[], object> Deserializer { get; set; } = Defaults.Deserializer;

        public bool HandleGetErrors { get; set; } = false;
        public bool HandleSetErrors { get; set; } = false;
        public bool HandleRemoveErrors { get; set; } = false;
        public bool HandleRefreshErrors { get; set; } = false;

        public event EventHandler<ErrorEventArgs> OnGetError;
        public event EventHandler<ErrorEventArgs> OnSetError;
        public event EventHandler<ErrorEventArgs> OnRemoveError;
        public event EventHandler<ErrorEventArgs> OnRefreshError;

        internal bool GetError(Exception exception)
        {
            try
            {
                this.OnGetError?.Invoke(this, new ErrorEventArgs(exception));
            }
            catch
            {
            }

            return this.HandleGetErrors;
        }

        internal bool SetError(Exception exception)
        {
            try
            {
                this.OnSetError?.Invoke(this, new ErrorEventArgs(exception));
            }
            catch
            {
            }

            return this.HandleSetErrors;
        }

        internal bool RemoveError(Exception exception)
        {
            try
            {
                this.OnRemoveError?.Invoke(this, new ErrorEventArgs(exception));
            }
            catch
            {
            }

            return this.HandleRemoveErrors;
        }

        internal bool RefreshError(Exception exception)
        {
            try
            {
                this.OnRefreshError?.Invoke(this, new ErrorEventArgs(exception));
            }
            catch
            {
            }

            return this.HandleRefreshErrors;
        }
    }
}