using System;

namespace Comminity.Extensions.Caching.Common
{
    public class CacheBase<TCacheInstance,TOptions>
            where TOptions : CommonCacheOptions<TCacheInstance>
    {
        public TOptions Options { get; }

        public CacheBase(TOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected string EnsureCorrectKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return this.Options.FullKeyFactory(typeof(TCacheInstance), key);
        }
    }
}