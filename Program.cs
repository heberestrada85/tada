using System;
using System.IO;
using Microsoft.AspNetCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace Tada
{
    public class Program
    {
        /// <param name="args"></param>

        public static async Task Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();

            using (IServiceScope scope = host.Services.CreateScope())
            {
                try
                {
                    IServiceProvider services = scope.ServiceProvider;
                    //await ApplicationDbContextSeed.SeedAsync(services);
                }
                catch (Exception ex)
                {
                    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error on migrate database.");
                }
            }
            await host.RunAsync();
        }

        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower(), "development", StringComparison.InvariantCultureIgnoreCase);
                    var envFile = "appsettings.json";
                    if (isDevelopment)
                        envFile = "appsettings.Development.json";

                    config.AddJsonFile(envFile,
                        optional: true,
                        reloadOnChange: false);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseWebRoot("public")
                .UseUrls("http://*:5001")
                .ConfigureLogging((c, l) =>
                {
                    var isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower(), "development", StringComparison.InvariantCultureIgnoreCase);
                    l.AddConfiguration(c.Configuration);
                })
                .UseStartup<Startup>();
    }
}
