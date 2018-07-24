using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comminity.Extensions.Caching.AppMetrics;
using Comminity.Extensions.Caching.Memory;
using Comminity.Extensions.Caching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Redis;

namespace Comminity.Extensions.Caching.Sample
{
    public class OnlyMemory { }

    public class OnlyDistributed { }

    public class Combined { }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCombinedCache<Combined>(options =>
            {
                options.AddDefaultMemoryCache();
                options.AddRedisDistributedCache(new RedisCacheOptions() { InstanceName = "redis", Configuration = "redis" });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
