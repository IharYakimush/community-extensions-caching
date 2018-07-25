using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Community.Extensions.Caching.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureMetricsWithDefaults(builder => builder.Build())
                .UseMetricsEndpoints()
                .UseStartup<Startup>();
    }
}
