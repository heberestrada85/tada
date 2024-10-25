using System;
using Tada.Domain.Entities;
using Tada.Application.Interface;
using Tada.Application.Extensions;
using Tada.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Tada.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Tada.Infrastructure.Persistence;
using Tada.Infrastructure.Identity.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using Tada.Application.Policies;
using Tada.Application.Models;
using Tada.Application.Helpers;
using Tada.Application.Constants;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Tada.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter((category, level) =>
                        category == DbLoggerCategory.Database.Command.Name
                        && level == LogLevel.Information)
                    .AddConsole();
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("DefaultConnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                    )
                    .LogTo(Console.WriteLine,LogLevel.Information)
                    .UseLoggerFactory(loggerFactory)
                );

            services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                                                .RequireAuthenticatedUser()
                                                .Build();
            });

            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IIdentityRoleService, IdentityRoleService>();
            services.AddSingleton<IJwtTokenHandler, JwtTokenHandler>();
            services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>();
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<ITokenFactory, TokenFactory>();

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

            services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtAppSettings.Issuer,
                    ValidAudience = jwtAppSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAppSettings.SecretKey)),
                };
                options.SaveToken = true;
                options.UseSecurityTokenValidators = true;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // Breakpoint aquí para ver por qué falla
                        Console.WriteLine($"Error de autenticación: {context.Exception}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                       var identity= context.Principal;
                        if (identity == null)
                            context.Fail("User invalid");
                        else
                        {
                            foreach (var claim in identity.Claims)
                            {
                                Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
                            }
                        }
                        return Task.CompletedTask;
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
