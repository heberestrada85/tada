using MediatR;
using AutoMapper;
using System.Reflection;
using Tada.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Tada.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<SignInManager<ApplicationUser>, SignInManager<ApplicationUser>>();
            services.AddScoped<RoleManager<ApplicationRole>, AspNetRoleManager<ApplicationRole>>();

            return services;
        }
    }
}
