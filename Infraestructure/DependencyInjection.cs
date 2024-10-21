using System;
using System.Reflection;
using System.Collections.Generic;
using Tada.Application.Interface;
using Tada.Application.Extensions;
using Tada.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tada.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;
using Tada.Infrastructure.Identity.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tada.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    )
                );

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());

            return services;
        }

        public static IServiceCollection AddJwtInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            JwtIssuerOptions jwtAppSettings = new JwtIssuerOptions
            {
                Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                SecretKey = jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)]
            };

            services.TryAddSingleton(jwtAppSettings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtAppSettings.SignInKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwtAppSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAppSettings.Audience,
                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                       var t= context.Principal;
                    }
                };
            });

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IJwtTokenHandler, JwtTokenHandler>();
            services.AddTransient<IJwtTokenValidator, JwtTokenValidator>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            services.AddTransient<IBasicAuthenticator, JwtAuthenticator>();
            services.AddTransient<IJwtAuthenticator, JwtAuthenticator>();
            return services;
        }
    }
}
