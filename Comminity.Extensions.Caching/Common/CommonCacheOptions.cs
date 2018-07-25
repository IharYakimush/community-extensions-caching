using System;

namespace Comminity.Extensions.Caching.Common
{
    public class CommonCacheOptions<TCacheInstance>
    {
        public Func<Type, string, string> FullKeyFactory { get; set; } = Defaults.FinalKeyFactoryForCacheInstance;
    }
}