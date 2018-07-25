using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Comminity.Extensions.Caching.Sample.Controllers
{
    [Route("/")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMemoryCache<MyCache> mem;
        private readonly IDistributedCache<MyCache> dist;
        private readonly ICombinedCache<MyCache> comb;
        private readonly Random rnd = new Random();

        public ValuesController(
            IMemoryCache<MyCache> onlyMemoryCache,
            IDistributedCache<MyCache> onlyDistributedCache, 
            ICombinedCache<MyCache> combinedCache)
        {
            mem = onlyMemoryCache ?? throw new ArgumentNullException(nameof(onlyMemoryCache));
            dist = onlyDistributedCache ?? throw new ArgumentNullException(nameof(onlyDistributedCache));
            comb = combinedCache ?? throw new ArgumentNullException(nameof(combinedCache));
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            string dateString = DateTime.UtcNow.ToLongDateString();
            return new string[]
            {
                this.mem.GetOrSetValue("v1", () => "v1mem-" + rnd.Next(1000000) + "-" + dateString,
                    new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)}),

                this.dist.GetOrSetValue("v1", () => "v1dist-" + rnd.Next(1000000) + "-" + dateString,
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)}),

                this.comb.GetOrSetValue("v1", () => "v1comb-" + rnd.Next(1000000) + "-" + dateString,
                    new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)},
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)}),

                this.comb.GetOrSetValue("v2", () => "v2comb-" + rnd.Next(1000000) + "-" + dateString,
                    new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)},
                    new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)})
            };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
