using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Comminity.Extensions.Caching.Tests
{
    public class UnitTests
    {
        [Fact]
        public async Task MemoryAndDistributed()
        {
            Mock<IDistributedCache> dist = new Mock<IDistributedCache>();
            dist.Setup(dc => dc.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<byte[]>(null));

            Mock<IMemoryCache> mem = new Mock<IMemoryCache>();
            mem.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns<object>(k =>
            {
                NullCacheEntry nullCacheEntry = new NullCacheEntry(k);
                return nullCacheEntry;
            });

            IServiceCollection services = new ServiceCollection();
            services.AddCombinedCache<UnitTests>(options =>
            {
                options.DistributedCache = dist.Object;
                options.MemoryCache = mem.Object;
            });

            ICombinedCache<UnitTests> cache = services.BuildServiceProvider().GetRequiredService<ICombinedCache<UnitTests>>();

            string str = await cache.GetOrAddAsync(AllowedCaches.All, "key", () => Task.FromResult("qwe"));

            Assert.Equal("qwe", str);
            string c;
            
            mem.Verify(mc => mc.CreateEntry("83A02F4BE2CA6C11D6E7AFF5D2D7FB45_13F231799E6146A6BA695C414EBEC71E_key"), Times.Exactly(1));
            dist.Verify(dc => dc.GetAsync("83A02F4BE2CA6C11D6E7AFF5D2D7FB45_13F231799E6146A6BA695C414EBEC71E_key", It.IsAny<CancellationToken>()),Times.Exactly(1));
        }
    }
}
