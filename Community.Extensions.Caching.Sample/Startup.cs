using Community.Extensions.Caching.AppMetrics;
using Community.Extensions.Caching.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Community.Extensions.Caching.Sample
{
    public class MyCache { }


    public class Startup
    {
        class MyCache1 { };

        class MyCache2 { };
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register memory cache
            services.AddMemoryCache<MyCache>();

            // Allow to collect App.Metrics for memory cache
            services.AddMetricsToMemoryCache<MyCache>();

            // Register Redis distributed cache
            services.AddRedisDistributedCache<MyCache>(options =>
            {
                options.Configuration = "redis";
                options.InstanceName = "redis";
            });

            // Allow to collect App.Metrics for distributed cache
            services.AddMetricsToDistributedCache<MyCache>();

            // Register combined cache
            services.AddCombinedCache<MyCache>();

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
