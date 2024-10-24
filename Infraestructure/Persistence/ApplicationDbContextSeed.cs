using System;
using MediatR;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Tada.Application.Models;
using Tada.Application.Helpers;
using Tada.Application.Interface;
using Tada.Application.Constants;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tada.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            try
            {
                UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                RoleManager<ApplicationRole> roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
                IDateTime dateTimeService = services.GetRequiredService<IDateTime>();
                ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
                var isDevelopment = services.GetRequiredService<IWebHostEnvironment>().IsDevelopment();
                IMediator mediator = services.GetRequiredService<IMediator>();
                CultureInfo culture = new CultureInfo("es-MX");
                try
                {
                    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Count() > 0)
                    {
                        context.Database.Migrate();
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

                var admin = await userManager.FindByNameAsync("admin");
                if (admin == null)
                {
                    ApplicationUser applicationUser = new ApplicationUser();
                    applicationUser.UserName = "development@dev.com";
                    applicationUser.Email = "development@dev.com";
                    applicationUser.Firstname = "Admin";
                    applicationUser.Surname = "Tada";
                    applicationUser.IdDepartment = 0;

                    IdentityResult result = await userManager.CreateAsync(applicationUser, "admin");

                    if (result.Succeeded)
                        Console.WriteLine("User created");
                    else
                        foreach (var error in result.Errors)
                            Console.WriteLine($"Error: {error.Code} - {error.Description} {error.Code}");

                    admin = await userManager.FindByNameAsync("development@dev.com");
                }

                ApplicationRole role = await roleManager.FindByNameAsync("admin");

                if (role == null)
                {
                    role = new ApplicationRole();
                    role.Name = "admin";
                    role.Description = "admin";

                    await roleManager.CreateAsync(role);

                    ICollection<Permissions> claims = PermissionHelper.GetPermissions();

                    foreach (Permissions permissions in claims)
                    {
                        await roleManager.AddClaimAsync(role, new Claim(ClaimsExtends.Authorization, permissions.Name));
                    }

                    await roleManager.UpdateAsync(role);
                }

                var roles = await userManager.GetRolesAsync(admin);
                bool isAdmin = false;

                foreach (string roleAdmin in roles)
                {
                    if (roleAdmin == "admin")
                        isAdmin = true;
                }

                if (!isAdmin)
                    await userManager.AddToRoleAsync(admin, "admin");

            }catch(Exception ex)
            {
                Console.WriteLine("------------- ERROR WHEN MIGRATING DATABASE -------------");
                Console.WriteLine(ex);
                Console.WriteLine("---------------------------------------------------------");
            }
        }
    }
}
