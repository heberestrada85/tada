using Tada.Application;
using System.Reflection;
using Tada.Infrastructure;
using Tada.Domain.Entities;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.ResponseCompression;

namespace Tada
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }
        readonly string CorsPolicy = "_CorsPolicy";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddJwtInfrastructure(Configuration);
            services.AddScoped<UserManager<ApplicationUser>, AspNetUserManager<ApplicationUser>>();
            services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();
            services.AddHttpContextAccessor();
            services.AddHealthChecks();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                builder =>
                {
                   builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new string[] { "image/svg+xml", "application/pdf", "application/json", "application/javascript", "application/rtf", "application/xhtml+xml", "application/xml", "text/*", "model/gltf+json" };
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddMvc();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();
            app.UseCors(CorsPolicy);
            app.UseHealthChecks("/health");
            app.UseResponseCompression();

            app.UseMvc();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Accounts}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        public string GetAssemblyVersion() =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
    }
}
