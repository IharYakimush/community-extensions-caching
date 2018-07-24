using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using System;

namespace Comminity.Extensions.Caching.Redis
{
    public static class BuilderExtensions
    {
        public static CombinedCacheOptions<TCacheInstance> AddRedisDistributedCache<TCacheInstance>(this CombinedCacheOptions<TCacheInstance> options, RedisCacheOptions redisCacheOptions)
        {
            if (redisCacheOptions == null)
            {
                throw new ArgumentNullException(nameof(redisCacheOptions));
            }

            options.DistributedCache = new RedisCache(Options.Create(redisCacheOptions));

            return options;
        }
    }
}
